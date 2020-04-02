# Mount & Blade Bannerlord Trade Improvements Mod

Currently the mod shows whether the items are profitable to buy or sell. The game already had this functionality displayed in the tooltip of an item but it is more convenient to see it straight away.

It overrides the `InventoryGauntletScreenOverride` class with a mirror like class that sends the _dataSource to the `MainApp` which changes the displayed fields. It is a mirror because the original class has all the needed fields as private.

A better way of doing this would be to override the _dataSource type `SPInventoryVM` and do the changes on refresh. However, a simple class `SPInventoryVMOverride` that extends `SPInventoryVM` with no extra logic caused tooltips and other ui elements to misbehave.

## Features ideas
- ability to sort the items by margin
- trade skill perk to switch the margins view on