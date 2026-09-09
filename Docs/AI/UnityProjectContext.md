# Unity Project Context

<!-- unity-onboarding:generated:start -->

- Last analyzed: 2026-09-09
- Unity: 6000.0.58f2 (Unity 6 LTS line)
- Rendering: Built-in Render Pipeline, 2D
- Input: Legacy Input Manager for the starter prototype
- Architecture: Small component-based prototype; deterministic run, mana, and affinity rules are plain C# classes
- Startup: `Assets/Editor/ProjectSetup.cs` generates `Assets/Scenes/Main.unity` on first import and adds it to Build Settings
- Tests: Unity Test Framework with EditMode tests under `Assets/Tests/EditMode`
- MCP: No Unity Editor MCP package or client configuration detected
- Current scope: Movement, basic melee attack, mana regeneration, health, enemy chase, three character presets, three affinities, ten-room progress
- Validation: Static inspection only; Unity Editor is unavailable in this workspace
- Important unknowns: final art direction, exact class skills, room layouts, artifact roster, balance targets, target platform

<!-- unity-onboarding:generated:end -->
