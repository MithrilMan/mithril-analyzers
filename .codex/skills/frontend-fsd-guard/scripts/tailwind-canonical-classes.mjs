import fs from 'node:fs/promises';
import path from 'node:path';
import process from 'node:process';
import { createRequire } from 'node:module';

const DEFAULT_SOURCE_EXTENSIONS = new Set(['.ts', '.tsx', '.js', '.jsx']);
const DEFAULT_CLASS_ATTRIBUTE_NAMES = new Set(['class', 'className']);
const DEFAULT_CLASS_UTILITY_CALLS = new Set(['cn', 'clsx', 'classNames', 'twMerge', 'cva']);
const DEFAULT_CSS_PATH = path.join('src', 'index.css');
const DEFAULT_SOURCE_ROOT = 'src';
const VALUE_TOLERANCE = 0.0001;

let ts;
let loadDesignSystem;

function printUsage() {
  console.log(`Tailwind v4 canonical-class fixer

Usage:
  node tailwind-canonical-classes.mjs [--check|--write] [--css path] [--src dir] [--callee name]

Options:
  --check         Report canonicalization findings and exit with code 1 when changes are needed.
  --write         Apply the canonicalization edits in place.
  --css <path>    Tailwind entry CSS file relative to the current working directory. Default: src/index.css
  --src <dir>     Source directory to scan. Repeat to include multiple roots. Default: src
  --callee <name> Additional class-composition helper to inspect. Repeatable.
  --help          Show this help text.
`);
}

function parseArgs(argv) {
  const sourceRoots = [];
  const extraCalleeFunctions = [];
  let cssPath = DEFAULT_CSS_PATH;
  let write = false;
  let explicitCheck = false;

  for (let index = 0; index < argv.length; index += 1) {
    const arg = argv[index];

    switch (arg) {
      case '--write':
        write = true;
        break;
      case '--check':
        explicitCheck = true;
        break;
      case '--css':
        index += 1;
        cssPath = argv[index] ?? cssPath;
        break;
      case '--src':
        index += 1;
        if (argv[index]) {
          sourceRoots.push(argv[index]);
        }
        break;
      case '--callee':
        index += 1;
        if (argv[index]) {
          extraCalleeFunctions.push(argv[index]);
        }
        break;
      case '--help':
      case '-h':
        printUsage();
        process.exit(0);
        break;
      default:
        throw new Error(`Unknown argument: ${arg}`);
    }
  }

  return {
    write,
    check: explicitCheck || !write,
    cssPath,
    sourceRoots: sourceRoots.length > 0 ? sourceRoots : [DEFAULT_SOURCE_ROOT],
    calleeFunctions: new Set([...DEFAULT_CLASS_UTILITY_CALLS, ...extraCalleeFunctions]),
  };
}

function loadWorkspaceDependencies(cwd) {
  const workspaceRequire = createRequire(path.join(cwd, '__tailwind-canonical-classes__.cjs'));

  try {
    ts = workspaceRequire('typescript');
  } catch (error) {
    throw new Error(
      `Missing workspace dependency "typescript" from ${cwd}. Install it in the target frontend before running this script.`,
      { cause: error },
    );
  }

  try {
    ({ __unstable__loadDesignSystem: loadDesignSystem } = workspaceRequire('@tailwindcss/node'));
  } catch (error) {
    throw new Error(
      `Missing workspace dependency "@tailwindcss/node" from ${cwd}. Install it in the target frontend before running this script.`,
      { cause: error },
    );
  }
}

async function walk(dir, sourceExtensions) {
  const entries = await fs.readdir(dir, { withFileTypes: true });
  const files = [];

  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);

    if (entry.isDirectory()) {
      files.push(...await walk(fullPath, sourceExtensions));
      continue;
    }

    if (sourceExtensions.has(path.extname(entry.name))) {
      files.push(fullPath);
    }
  }

  return files;
}

function formatNumber(value) {
  const normalized = Math.abs(value) < VALUE_TOLERANCE ? 0 : value;
  const rounded = Number(normalized.toFixed(6));

  if (Number.isInteger(rounded)) {
    return `${rounded}`;
  }

  return `${rounded}`.replace(/\.0+$/, '').replace(/(\.\d*?[1-9])0+$/, '$1');
}

function nearlyEqual(left, right) {
  return Math.abs(left - right) <= VALUE_TOLERANCE;
}

function normalizeCssValue(value, rootFontSize) {
  const trimmed = value.trim().toLowerCase();
  const numberMatch = trimmed.match(/^-?(?:\d+\.?\d*|\.\d+)$/);

  if (numberMatch) {
    return {
      kind: 'number',
      value: Number(trimmed),
      key: `number:${formatNumber(Number(trimmed))}`,
    };
  }

  const lengthMatch = trimmed.match(/^(-?(?:\d+\.?\d*|\.\d+))(px|rem|em)$/);
  if (!lengthMatch) {
    return null;
  }

  const amount = Number(lengthMatch[1]);
  const unit = lengthMatch[2];

  if (unit === 'em') {
    return {
      kind: 'em',
      value: amount,
      key: `em:${formatNumber(amount)}`,
    };
  }

  const absoluteValue = unit === 'px' ? amount : amount * rootFontSize;
  return {
    kind: 'absolute-length',
    value: absoluteValue,
    key: `absolute-length:${formatNumber(absoluteValue)}`,
  };
}

function getExpressionName(node) {
  if (ts.isIdentifier(node)) {
    return node.text;
  }

  if (ts.isPropertyAccessExpression(node)) {
    return node.name.text;
  }

  return null;
}

function isClassContext(node, calleeFunctions) {
  let current = node.parent;

  while (current) {
    if (ts.isJsxAttribute(current) && ts.isIdentifier(current.name) && DEFAULT_CLASS_ATTRIBUTE_NAMES.has(current.name.text)) {
      return true;
    }

    if (ts.isCallExpression(current)) {
      const calleeName = getExpressionName(current.expression);
      if (calleeName && calleeFunctions.has(calleeName)) {
        return true;
      }
    }

    current = current.parent;
  }

  return false;
}

function getVariantPrefix(parsedCandidate, designSystem) {
  if (!Array.isArray(parsedCandidate.variants) || parsedCandidate.variants.length === 0) {
    return '';
  }

  const variants = [...parsedCandidate.variants].reverse().map(variant => designSystem.printVariant(variant));
  return `${variants.join(':')}:`;
}

function buildCandidateToken(parsedCandidate, root, suffix, designSystem) {
  return `${getVariantPrefix(parsedCandidate, designSystem)}${parsedCandidate.important ? '!' : ''}${root}-${suffix}`;
}

function isValidCandidate(candidate, designSystem, cache) {
  const cached = cache.get(candidate);
  if (cached !== undefined) {
    return cached;
  }

  const valid = designSystem.candidatesToCss([candidate])[0] !== null;
  cache.set(candidate, valid);
  return valid;
}

function buildThemeAliasEntries(designSystem, rootFontSize) {
  const aliasEntries = [];

  const themeFamilies = [
    { keyPrefix: '--radius-', themePrefix: 'radius', allow: alias => !alias.includes('--') },
    {
      keyPrefix: '--text-',
      themePrefix: 'text',
      allow: alias => !alias.includes('--') && !alias.startsWith('shadow-'),
    },
    { keyPrefix: '--tracking-', themePrefix: 'tracking', allow: alias => !alias.includes('--') },
    { keyPrefix: '--leading-', themePrefix: 'leading', allow: alias => !alias.includes('--') },
  ];

  for (const family of themeFamilies) {
    for (const themeKey of designSystem.theme.values.keys()) {
      if (!themeKey.startsWith(family.keyPrefix)) {
        continue;
      }

      const alias = themeKey.slice(family.keyPrefix.length);
      if (!family.allow(alias)) {
        continue;
      }

      const resolvedValue = designSystem.resolveThemeValue(`${family.themePrefix}.${alias}`);
      const normalized = resolvedValue ? normalizeCssValue(resolvedValue, rootFontSize) : null;

      if (!normalized) {
        continue;
      }

      aliasEntries.push({
        key: normalized.key,
        suffix: alias,
      });
    }
  }

  return aliasEntries;
}

function createCanonicalizationContext(designSystem, rootFontSize, calleeFunctions) {
  const spacingToken = designSystem.theme.values.get('--spacing');
  const normalizedSpacing = spacingToken ? normalizeCssValue(spacingToken.value, rootFontSize) : null;

  return {
    designSystem,
    calleeFunctions,
    rootFontSize,
    candidateCache: new Map(),
    validityCache: new Map(),
    spacingBase: normalizedSpacing?.kind === 'absolute-length' ? normalizedSpacing.value : null,
    themeAliasEntries: buildThemeAliasEntries(designSystem, rootFontSize),
  };
}

function buildSignedRoot(parsedCandidate, negativeValue) {
  if (!negativeValue) {
    return parsedCandidate.root;
  }

  return parsedCandidate.root.startsWith('-')
    ? parsedCandidate.root.slice(1)
    : `-${parsedCandidate.root}`;
}

function tryExactReplacementSuffix(parsedCandidate, suffix, negativeValue, context) {
  const candidate = buildCandidateToken(
    parsedCandidate,
    buildSignedRoot(parsedCandidate, negativeValue),
    suffix,
    context.designSystem,
  );

  return isValidCandidate(candidate, context.designSystem, context.validityCache) ? candidate : null;
}

function tryNumericCanonicalization(parsedCandidate, normalizedValue, context) {
  if (normalizedValue.kind !== 'number') {
    return null;
  }

  return tryExactReplacementSuffix(
    parsedCandidate,
    formatNumber(Math.abs(normalizedValue.value)),
    normalizedValue.value < 0,
    context,
  );
}

function trySpacingCanonicalization(parsedCandidate, normalizedValue, context) {
  if (normalizedValue.kind !== 'absolute-length' || context.spacingBase === null || nearlyEqual(context.spacingBase, 0)) {
    return null;
  }

  const absoluteValue = Math.abs(normalizedValue.value);

  if (nearlyEqual(absoluteValue, 1)) {
    const pxCandidate = tryExactReplacementSuffix(parsedCandidate, 'px', normalizedValue.value < 0, context);
    if (pxCandidate) {
      return pxCandidate;
    }
  }

  const multiplier = absoluteValue / context.spacingBase;
  if (!Number.isFinite(multiplier)) {
    return null;
  }

  return tryExactReplacementSuffix(
    parsedCandidate,
    formatNumber(multiplier),
    normalizedValue.value < 0,
    context,
  );
}

function tryThemeAliasCanonicalization(parsedCandidate, normalizedValue, context) {
  const comparisonKey = normalizedValue.kind === 'number'
    ? `number:${formatNumber(Math.abs(normalizedValue.value))}`
    : normalizedValue.kind === 'absolute-length'
      ? `absolute-length:${formatNumber(Math.abs(normalizedValue.value))}`
      : `em:${formatNumber(Math.abs(normalizedValue.value))}`;

  for (const entry of context.themeAliasEntries) {
    if (entry.key !== comparisonKey) {
      continue;
    }

    const candidate = tryExactReplacementSuffix(parsedCandidate, entry.suffix, normalizedValue.value < 0, context);
    if (candidate) {
      return candidate;
    }
  }

  return null;
}

function tryHeuristicCanonicalization(token, context) {
  const parsedCandidates = context.designSystem.parseCandidate(token);
  if (parsedCandidates.length !== 1) {
    return token;
  }

  const parsedCandidate = parsedCandidates[0];
  if (
    parsedCandidate.kind !== 'functional'
    || parsedCandidate.modifier !== null
    || !parsedCandidate.value
    || parsedCandidate.value.kind !== 'arbitrary'
  ) {
    return token;
  }

  const normalizedValue = normalizeCssValue(parsedCandidate.value.value, context.rootFontSize);
  if (!normalizedValue) {
    return token;
  }

  return (
    tryNumericCanonicalization(parsedCandidate, normalizedValue, context)
    ?? trySpacingCanonicalization(parsedCandidate, normalizedValue, context)
    ?? tryThemeAliasCanonicalization(parsedCandidate, normalizedValue, context)
    ?? token
  );
}

function canonicalizeToken(token, context) {
  const cached = context.candidateCache.get(token);
  if (cached) {
    return cached;
  }

  const apiCanonical = context.designSystem.canonicalizeCandidates([token])[0] ?? token;
  const canonical = apiCanonical !== token
    ? apiCanonical
    : tryHeuristicCanonicalization(token, context);

  context.candidateCache.set(token, canonical);
  return canonical;
}

function canonicalizeLiteral(text, context) {
  const tokens = [...text.matchAll(/[^\s]+/g)];
  if (tokens.length === 0) {
    return null;
  }

  let candidateCount = 0;
  let changed = false;
  let output = '';
  let cursor = 0;

  for (const match of tokens) {
    const token = match[0];
    const index = match.index ?? 0;
    const isTailwindCandidate = context.designSystem.parseCandidate(token).length > 0;
    const canonical = isTailwindCandidate ? canonicalizeToken(token, context) : token;

    output += text.slice(cursor, index);
    output += canonical;
    cursor = index + token.length;

    if (isTailwindCandidate) {
      candidateCount += 1;
      changed ||= canonical !== token;
    }
  }

  if (candidateCount === 0 || !changed) {
    return null;
  }

  output += text.slice(cursor);
  return output;
}

function collectStringEdits(sourceFile, context) {
  const edits = [];

  function visit(node) {
    if ((ts.isStringLiteral(node) || ts.isNoSubstitutionTemplateLiteral(node)) && isClassContext(node, context.calleeFunctions)) {
      const nextText = canonicalizeLiteral(node.text, context);

      if (nextText !== null) {
        edits.push({
          start: node.getStart(sourceFile) + 1,
          end: node.getEnd() - 1,
          nextText,
          line: sourceFile.getLineAndCharacterOfPosition(node.getStart(sourceFile)).line + 1,
        });
      }
    }

    ts.forEachChild(node, visit);
  }

  visit(sourceFile);
  return edits.sort((left, right) => right.start - left.start);
}

function applyEdits(content, edits) {
  let nextContent = content;

  for (const edit of edits) {
    nextContent = `${nextContent.slice(0, edit.start)}${edit.nextText}${nextContent.slice(edit.end)}`;
  }

  return nextContent;
}

function getSourceKind(filePath) {
  switch (path.extname(filePath)) {
    case '.tsx':
      return ts.ScriptKind.TSX;
    case '.ts':
      return ts.ScriptKind.TS;
    case '.jsx':
      return ts.ScriptKind.JSX;
    default:
      return ts.ScriptKind.JS;
  }
}

async function main() {
  const { check, write, cssPath, sourceRoots, calleeFunctions } = parseArgs(process.argv.slice(2));
  const cwd = process.cwd();
  loadWorkspaceDependencies(cwd);

  const absoluteCssPath = path.resolve(cwd, cssPath);
  const absoluteSourceRoots = sourceRoots.map(sourceRoot => path.resolve(cwd, sourceRoot));

  const css = await fs.readFile(absoluteCssPath, 'utf8');
  const designSystem = await loadDesignSystem(css, { base: path.dirname(absoluteCssPath) });
  const context = createCanonicalizationContext(designSystem, DEFAULT_ROOT_FONT_SIZE, calleeFunctions);

  const files = [];
  for (const sourceRoot of absoluteSourceRoots) {
    files.push(...await walk(sourceRoot, DEFAULT_SOURCE_EXTENSIONS));
  }

  let changedFiles = 0;
  let changedLiterals = 0;
  const findings = [];

  for (const filePath of files) {
    const content = await fs.readFile(filePath, 'utf8');
    const sourceFile = ts.createSourceFile(filePath, content, ts.ScriptTarget.Latest, true, getSourceKind(filePath));
    const edits = collectStringEdits(sourceFile, context);

    if (edits.length === 0) {
      continue;
    }

    changedFiles += 1;
    changedLiterals += edits.length;
    findings.push({
      filePath: path.relative(cwd, filePath),
      lines: edits.map(edit => edit.line),
    });

    if (write) {
      const nextContent = applyEdits(content, edits);
      if (nextContent !== content) {
        await fs.writeFile(filePath, nextContent, 'utf8');
      }
    }
  }

  if (changedFiles === 0) {
    console.log('Tailwind canonical classes are already clean.');
    return;
  }

  const summary = findings
    .map(item => `- ${item.filePath} (${item.lines.join(', ')})`)
    .join('\n');

  console.log(
    `${write ? 'Updated' : 'Found'} ${changedLiterals} canonical-class change(s) across ${changedFiles} file(s):\n${summary}`,
  );

  if (check && !write) {
    process.exitCode = 1;
  }
}

const DEFAULT_ROOT_FONT_SIZE = 16;

await main();
