import fs from 'node:fs';
import path from 'node:path';

const DEFAULT_EXTENSIONS = ['.tsx'];
const DEFAULT_WARNING_LINES = 300;
const DEFAULT_MAX_LINES = 400;
const SKIPPED_DIRECTORIES = new Set(['.git', '.turbo', 'build', 'dist', 'node_modules']);

const options = parseArguments(process.argv.slice(2));
const projectRoot = process.cwd();
const configPath = options.config ? path.resolve(projectRoot, options.config) : null;
const config = configPath ? loadConfig(configPath) : {};

const sourceRoot = path.resolve(projectRoot, options.root ?? config.root ?? 'src');
const extensions = normalizeExtensions(options.extensions ?? config.extensions ?? DEFAULT_EXTENSIONS);
const warningLines = parsePositiveInteger(options.warningLines ?? config.warningLines ?? DEFAULT_WARNING_LINES, 'warningLines');
const maxLines = parsePositiveInteger(options.maxLines ?? config.maxLines ?? DEFAULT_MAX_LINES, 'maxLines');
const excludeMatchers = normalizeExcludePatterns(config.exclude ?? []);
const overrides = normalizeOverrides(config.overrides ?? {});

if (warningLines >= maxLines) {
  fail(`Invalid thresholds: warningLines (${warningLines}) must be lower than maxLines (${maxLines}).`);
}

if (!fs.existsSync(sourceRoot) || !fs.statSync(sourceRoot).isDirectory()) {
  fail(`Source root not found or not a directory: ${path.relative(projectRoot, sourceRoot) || sourceRoot}`);
}

const files = [];
walk(sourceRoot, files, extensions);

const warningZone = [];
const violations = [];

for (const filePath of files.sort()) {
  const relativePath = toPosix(path.relative(projectRoot, filePath));
  const isExcluded = excludeMatchers.some(matcher => matcher.test(relativePath));

  if (isExcluded) {
    continue;
  }

  const source = fs.readFileSync(filePath, 'utf8');
  const lineCount = countLines(source);
  const override = overrides.get(relativePath);

  const effectiveWarningLines = override?.warningLines ?? warningLines;
  const effectiveMaxLines = override?.maxLines ?? maxLines;

  if (effectiveWarningLines >= effectiveMaxLines) {
    fail(`Invalid override for ${relativePath}: warningLines (${effectiveWarningLines}) must be lower than maxLines (${effectiveMaxLines}).`);
  }

  if (lineCount > effectiveWarningLines && lineCount <= effectiveMaxLines) {
    warningZone.push({
      relativePath,
      lineCount,
      effectiveWarningLines,
      effectiveMaxLines,
      reason: override?.reason ?? null,
    });
  }

  if (lineCount > effectiveMaxLines) {
    violations.push({
      relativePath,
      lineCount,
      effectiveMaxLines,
      reason: override?.reason ?? null,
    });
  }
}

if (warningZone.length === 0 && violations.length === 0) {
  console.log(
    `File-size guard passed for ${path.relative(projectRoot, sourceRoot) || sourceRoot} (${extensions.join(', ')}, warn>${warningLines}, fail>${maxLines}).`,
  );
  process.exit(0);
}

if (warningZone.length > 0) {
  console.log('Files in the warning zone:\n');
  for (const entry of warningZone) {
    console.log(`- ${entry.lineCount} lines: ${entry.relativePath} (warn>${entry.effectiveWarningLines}, fail>${entry.effectiveMaxLines})`);
    if (entry.reason) {
      console.log(`  override: ${entry.reason}`);
    }
  }
  console.log('');
}

if (violations.length > 0) {
  console.error('Files exceeding the hard limit:\n');
  for (const entry of violations) {
    console.error(`- ${entry.lineCount} lines: ${entry.relativePath} (fail>${entry.effectiveMaxLines})`);
    if (entry.reason) {
      console.error(`  override: ${entry.reason}`);
    }
  }
  console.error('');
  console.error('Split by ownership before adding more lines: keep route composition in pages, reusable page regions in widgets, focused interactions in features, and domain helpers in entities.');
  process.exit(1);
}

process.exit(0);

function walk(directoryPath, files, extensionsSet) {
  for (const entry of fs.readdirSync(directoryPath, { withFileTypes: true })) {
    if (entry.isDirectory()) {
      if (SKIPPED_DIRECTORIES.has(entry.name)) {
        continue;
      }

      walk(path.join(directoryPath, entry.name), files, extensionsSet);
      continue;
    }

    const extension = path.extname(entry.name);
    if (extensionsSet.has(extension)) {
      files.push(path.join(directoryPath, entry.name));
    }
  }
}

function countLines(source) {
  if (source.length === 0) {
    return 0;
  }

  return source.split(/\r\n|\r|\n/).length;
}

function loadConfig(configPath) {
  if (!fs.existsSync(configPath)) {
    fail(`Config file not found: ${path.relative(process.cwd(), configPath) || configPath}`);
  }

  try {
    return JSON.parse(fs.readFileSync(configPath, 'utf8'));
  } catch (error) {
    fail(`Unable to parse JSON config ${path.relative(process.cwd(), configPath) || configPath}: ${error.message}`);
  }
}

function normalizeExtensions(rawExtensions) {
  if (!Array.isArray(rawExtensions) || rawExtensions.length === 0) {
    fail('Expected "extensions" to be a non-empty array.');
  }

  return new Set(
    rawExtensions.map(extension => {
      if (typeof extension !== 'string' || extension.trim().length === 0) {
        fail('Each extension must be a non-empty string.');
      }

      return extension.startsWith('.') ? extension : `.${extension}`;
    }),
  );
}

function normalizeOverrides(rawOverrides) {
  if (rawOverrides === null || typeof rawOverrides !== 'object' || Array.isArray(rawOverrides)) {
    fail('Expected "overrides" to be an object map.');
  }

  const normalizedOverrides = new Map();

  for (const [rawPath, rawOverride] of Object.entries(rawOverrides)) {
    const relativePath = toPosix(rawPath);

    if (rawOverride === null || typeof rawOverride !== 'object' || Array.isArray(rawOverride)) {
      fail(`Override for ${relativePath} must be an object.`);
    }

    const maxOverride = rawOverride.maxLines ?? null;
    const warningOverride = rawOverride.warningLines ?? null;
    const reason = typeof rawOverride.reason === 'string' ? rawOverride.reason.trim() : '';

    if (maxOverride !== null && maxOverride !== undefined) {
      parsePositiveInteger(maxOverride, `overrides.${relativePath}.maxLines`);
    }

    if (warningOverride !== null && warningOverride !== undefined) {
      parsePositiveInteger(warningOverride, `overrides.${relativePath}.warningLines`);
    }

    if ((maxOverride !== null || warningOverride !== null) && reason.length === 0) {
      fail(`Override for ${relativePath} must include a non-empty "reason" so exceptions stay documented.`);
    }

    normalizedOverrides.set(relativePath, {
      maxLines: maxOverride === null || maxOverride === undefined ? undefined : Number(maxOverride),
      warningLines: warningOverride === null || warningOverride === undefined ? undefined : Number(warningOverride),
      reason: reason.length === 0 ? undefined : reason,
    });
  }

  return normalizedOverrides;
}

function normalizeExcludePatterns(rawExcludePatterns) {
  if (!Array.isArray(rawExcludePatterns)) {
    fail('Expected "exclude" to be an array of glob-like strings.');
  }

  return rawExcludePatterns.map(pattern => {
    if (typeof pattern !== 'string' || pattern.trim().length === 0) {
      fail('Each exclude entry must be a non-empty string.');
    }

    return globToRegExp(toPosix(pattern.trim()));
  });
}

function parseArguments(argumentsList) {
  const options = {};

  for (let index = 0; index < argumentsList.length; index += 1) {
    const token = argumentsList[index];
    const nextValue = argumentsList[index + 1];

    if (!token.startsWith('--')) {
      fail(`Unexpected argument: ${token}`);
    }

    switch (token) {
      case '--config':
        options.config = requireValue(token, nextValue);
        index += 1;
        break;
      case '--root':
        options.root = requireValue(token, nextValue);
        index += 1;
        break;
      case '--extensions':
        options.extensions = requireValue(token, nextValue)
          .split(',')
          .map(value => value.trim())
          .filter(Boolean);
        index += 1;
        break;
      case '--warning-lines':
        options.warningLines = requireValue(token, nextValue);
        index += 1;
        break;
      case '--max-lines':
        options.maxLines = requireValue(token, nextValue);
        index += 1;
        break;
      default:
        fail(`Unknown argument: ${token}`);
    }
  }

  return options;
}

function requireValue(flagName, value) {
  if (value === undefined || value.startsWith('--')) {
    fail(`Missing value for ${flagName}`);
  }

  return value;
}

function parsePositiveInteger(value, label) {
  const parsedValue = Number(value);

  if (!Number.isInteger(parsedValue) || parsedValue <= 0) {
    fail(`Expected ${label} to be a positive integer, received: ${value}`);
  }

  return parsedValue;
}

function toPosix(value) {
  return value.replace(/\\/g, '/');
}

function globToRegExp(pattern) {
  const escapedPattern = pattern.replace(/[.+^${}()|[\]\\]/g, '\\$&');
  const regexSource = escapedPattern
    .replace(/\*\*/g, '::DOUBLE_STAR::')
    .replace(/\*/g, '[^/]*')
    .replace(/::DOUBLE_STAR::/g, '.*');

  return new RegExp(`^${regexSource}$`);
}

function fail(message) {
  console.error(`File-size guard configuration error: ${message}`);
  process.exit(1);
}
