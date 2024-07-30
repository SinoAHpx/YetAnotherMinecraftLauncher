# YetAnotherMinecraftLauncher

## Config system anatomized

```mermaid
flowchart TB
	A1([Application Started])
	A2([Application Stopped])
	B1(Read Config)
	B2(Write Config)
	B3(Create Empty Config)

	
	A1 -- Config not found --> B3
	A1 -- Config found --> B1
	B1 & B3 -- Config changed --> B2 --> A2
	B1 & B3 -- Config unchanged --> A2
```

- `Round square` is for application lifetime
- `Square` is for user behavior and input
- `Round corner square` is for application response upon user behavior
