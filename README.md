# NOKIA3310JAM

## Space Ship

### States

- Outside an encounter
	- Navigation choice
		- Go around at a cost
		- Ignore (no reward no cost)
		- Take a risk (reward)
	- System status
		- Health
		- Engines
		- Weapons
		- Shields
	- Stats
	- Inventory

- In an encounter
	- Attack
		- Base attack
		- Special attack
			- Consumes special ammo (limited resource)
		- Boarding (after scan)
	- Defend
		- Repair
			- Boost all systems to a certain minimum or gain percentage
		- Shields
			- Raise shields for N turns
		- Repel boarders
			- Always works
		- Flee
			- Consumes fuel (limited resource)
		- Evasion
	- Utility
		- Reroute power
			- Stat transfer
		- Scan
			- Do it once
			- Flat bonus
	- Status
		- Self
			- Health
			- Engines
			- Weapons
			- Shields
		- Enemy (after scan)

- Post encounter
	- Choice (game will increase chance/amount of required resources)
		- Fuel
		- Repair ship from salvage
		- Special ammo
		- Stat boost

### Gameplay Loop

Navigation choice -> encounter -> post encounter -> navigation

### Inventory

- Special ammo
- Fuel

### Extensions

- Inventory
	- Special items from random encounters
	- Can be used in the outside an encounter state