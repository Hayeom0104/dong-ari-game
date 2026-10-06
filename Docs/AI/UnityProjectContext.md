# Unity Project Context

<!-- unity-onboarding:generated:start -->

- Last analyzed: 2026-10-07
- Unity: 6000.0.58f2 (Unity 6 LTS line)
- Rendering: Built-in Render Pipeline, 2D
- Input: Legacy Input Manager for the starter prototype
- Architecture: Small component-based prototype; deterministic run, mana, and affinity rules are plain C# classes
- Startup: `Assets/Editor/ProjectSetup.cs` generates `Assets/Scenes/Main.unity` on first import and adds it to Build Settings
- Tests: Unity Test Framework with EditMode tests under `Assets/Tests/EditMode`
- MCP: No Unity Editor MCP package or client configuration detected
- Current scope: Complete prototype loop with character selection, stat application, movement, attacks, enemy damage, mana regeneration, artifacts, HUD, elapsed timer, ten rooms, final boss, and results
- Validation: Static inspection only; Unity Editor is unavailable in this workspace
- Art: transparent Heroes/Monsters atlases in Resources/Art; 3 clips × 8 frames each, runtime SpriteWalkAnimator at 10fps; six monster types share three base clips. Existing Main scenes receive art at runtime.
- Important unknowns: final art direction, exact class skills, room layouts, artifact roster, balance targets, target platform

<!-- unity-onboarding:generated:end -->
