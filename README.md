# Kits
Create kits with custom items, cooldown, price, experience and vehicle.

## Features
* Works with Uconomy plugin to charge players for kits.
* Supports rich text formatting and custom message icon.
* Cooldowns are saved in database and are not lost after server restart.
* Kits can be created in-game using command.
* Global cooldown prevents players from using kits too often.
* Kits are saved in XML file and can be easily edited. Remember to reload the configuration after editing the file.

> **Note:** Admins bypass cooldowns and can use kits without any restrictions. Do not give `kits.admin` permission to normal players.

## Commands
* `/kit <name>` - Use the kit.
* `/kit <name> <player>` - Give the kit to another player. Requires `givekit` permission.
* `/kits` - List all available kits.
* `/ckit <name> <cooldown> [price] [experience] [vehicle]` - Create a new kit with items in your inventory. `price`, `experience`, `vehicle` are optional and if you want to skip them, use `0`.
* `/dkit <name>` - Delete a kit.
* `/rocket reload Kits` - Reload the configuration.

## Permissions
To grant access to the kit, add the permission `kit.<name>` to the player. For example, to give access to the kit named **Soldier**, add the permission `kit.soldier`.

```xml
<!-- Permissions that all players should have -->
<Permission Cooldown="0">kit</Permission>
<Permission Cooldown="0">kits</Permission>

<!-- Example if you want to give access to the kit named Soldier -->
<Permission Cooldown="0">kit.soldier</Permission>

<!-- Optional givekit permission. I don't recommend giving it to players -->
<Permission Cooldown="0">givekit</Permission>

<!-- Admin permission that grants access to all commands, kits and bypasses cooldowns. DO NOT GIVE IT TO NORMAL PLAYERS! -->
<Permission Cooldown="0">kits.admin</Permission>
```

## Configuration
```xml
<?xml version="1.0" encoding="utf-8"?>
<KitsConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <MessageColor>yellow</MessageColor>
  <MessageIconUrl>https://i.imgur.com/ceyPI5h.png</MessageIconUrl>
  <GlobalCooldown>30</GlobalCooldown>
</KitsConfiguration>
```

## Kits.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<KitsDatabase xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Kits>
    <Kit Name="Soldier" Cooldown="300" Price="100" Experience="100" VehicleId="93" VehicleName="Huey_Forest">
      <Items>
        <Item Id="253" Name="Alicepack" />
        <Item Id="310" Name="Forest Military Vest" />
        <Item Id="116" Name="PDW" Metadata="dgAAAAAAdQAGAB4CAWRkZGRk" />
        <Item Id="363" Name="Maplestrike" Metadata="kgAAAAAAAAAGAB4CAWRkZGRk" />
        <Item Id="394" Name="Dressing" Amount="5" />
        <Item Id="81" Name="MRE" Amount="2" />
        <Item Id="95" Name="Bandage" Amount="3" />
        <Item Id="469" Name="Canned Ham" Amount="3" />
        <Item Id="6" Name="Military Magazine" Amount="3" />
      </Items>
    </Kit>
  </Kits>
</KitsDatabase>
```

## Translations
```xml
<?xml version="1.0" encoding="utf-8"?>
<Translations xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Translation Id="KitCommandSyntax" Value="You must specify kit name. Use [[b]]/kits[[/b]] to see the list of available kits." />
  <Translation Id="KitCommandConsoleSyntax" Value="You must specify kit name and player you want to receive the kit." />
  <Translation Id="KitNotFound" Value="Kit [[b]]{0}[[/b]] not found." />
  <Translation Id="KitGlobalCooldown" Value="You have to wait [[b]]{0}[[/b]] before using any kit again." />
  <Translation Id="KitCooldown" Value="You have to wait [[b]]{0}[[/b]] before using kit [[b]]{1}[[/b]] again." />
  <Translation Id="KitNotEnoughMoney" Value="You can't afford to buy this kit for [[b]]${0}[[/b]] credits." />
  <Translation Id="KitReceived" Value="You received kit [[b]]{0}[[/b]]." />
  <Translation Id="KitReceivedFromSomeone" Value="You received kit [[b]]{0}[[/b]] from [[b]]{1}[[/b]]." />
  <Translation Id="KitGiven" Value="You have given kit [[b]]{0}[[/b]] to [[b]]{1}[[/b]]." />
  <Translation Id="KitAlreadyExists" Value="Kit [[b]]{0}[[/b]] already exists." />
  <Translation Id="CreateKitCommandSyntax" Value="Usage: /ckit [[name]] [[cooldown]] [price] [experience] [vehicle]" />
  <Translation Id="CreateKitNameInvalid" Value="Name must contain no special characters. [[b]]{0}[[/b]] is invalid." />
  <Translation Id="CreateKitCooldownNotNumber" Value="Cooldown must be a number. [[b]]{0}[[/b]] is invalid." />
  <Translation Id="CreateKitPriceNotNumber" Value="Price must be a number. [[b]]{0}[[/b]] is invalid." />
  <Translation Id="CreateKitUconomyNotInstalled" Value="You must install Uconomy plugin to create kits with prices." />
  <Translation Id="CreateKitExperienceNotNumber" Value="Experience must be a number. [[b]]{0}[[/b]] is invalid." />
  <Translation Id="CreateKitVehicleNotFound" Value="Vehicle [[b]]{0}[[/b]] not found." />
  <Translation Id="KitCreated" Value="Created kit [[b]]{0}[[/b]] with [[b]]{1}[[/b]] cooldown and [[b]]{2}[[/b]] items." />
  <Translation Id="NoKitsAvailable" Value="You don't have access to any kits." />
  <Translation Id="KitNameFormat" Value="[[b]]{0}[[/b]]" />
  <Translation Id="KitPriceFormat" Value="[${0}]" />
  <Translation Id="KitCooldownFormat" Value="({0})" />
  <Translation Id="KitsAvailable" Value="Your kits: {0}" />
  <Translation Id="DeleteKitCommandSyntax" Value="You must specify kit name." />
  <Translation Id="KitDeleted" Value="Deleted kit [[b]]{0}[[/b]]." />
  <Translation Id="KitNoPermission" Value="You don't have permission to use kit [[b]]{0}[[/b]]." />
  <Translation Id="KitOtherNoPermission" Value="You don't have permission to give kit to other players." />
  <Translation Id="PlayerNotFound" Value="Player {0} not found." />
  <Translation Id="Day" Value="1 day" />
  <Translation Id="Days" Value="{0} days" />
  <Translation Id="Hour" Value="1 hour" />
  <Translation Id="Hours" Value="{0} hours" />
  <Translation Id="Minute" Value="1 minute" />
  <Translation Id="Minutes" Value="{0} minutes" />
  <Translation Id="Second" Value="1 second" />
  <Translation Id="Seconds" Value="{0} seconds" />
  <Translation Id="Zero" Value="a moment" />
  <Translation Id="DayShort" Value="{0}d" />
  <Translation Id="HourShort" Value="{0}h" />
  <Translation Id="MinuteShort" Value="{0}m" />
  <Translation Id="SecondShort" Value="{0}s" />
  <Translation Id="KitAdminBypassPermission" Value="You have bypassed kit cooldown, because you are admin or have kits.admin permission." />
</Translations>
```