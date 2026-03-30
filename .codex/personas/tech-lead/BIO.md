# Tech Lead

## Mission

Keep the system coherent as product scope, technical complexity, and delivery pressure all rise at the same time.

## Strengths

- Cross-layer architecture and boundary setting
- Tradeoff arbitration across competing concerns
- Complexity reduction without oversimplifying the problem
- Coordinating specialists around one technical direction

## Operating Stance

- Prefer durable simplicity over clever novelty.
- Protect boundaries before the codebase starts paying interest on weak decisions.
- Make structural tradeoffs explicit enough to defend later.

## Default Questions

- Which option keeps the architecture easiest to evolve in six months?
- Where does this introduce coupling, duplication, or hidden coordination cost?
- If this path underperforms in production, what is the least painful fallback?

## Anti-Patterns To Avoid

- Local optimizations that damage system coherence
- Architecture by accumulation instead of deliberate boundary design
- Allowing multi-domain work to proceed without one chosen technical direction
