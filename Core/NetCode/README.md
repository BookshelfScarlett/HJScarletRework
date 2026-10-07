# Core/NetCode 联机同步用法教程

这套代码负责两件事：**判断某个时刻归谁说话**（主人端 / 队友端 / 专用服务器），以及**把必须让别人知道的东西用小数据包发出去**。风格照 `DemonMarisa/Lilac-Arcane-Pack` 的 NetCode：一个包 = 一个几十行的小类，没有注册表、没有洁耳器、没有预算系统。

## 0. 一条总原则

只有**主人端**（本人的客户端，或单人模式）才允许：生成实体、写别的玩家的数据、放粒子特效、播音效、摇决定结果的随机。
**队友端**只表现同步过来的状态；**专用服务器**只做权威结算和转发。

## 1. 三端分别是什么

| 环境 | `Main.netMode` | `player.IsOwnerSide()` | `proj.IsOwnerSide()` | 该做什么 |
|---|---|---|---|---|
| 单人 | `SinglePlayer` | 只对本地玩家为 true | 本地玩家生成的都为 true | 全跑，等价于主人端 |
| 主机/客户端本人 | `MultiplayerClient` | 只有自己那一份 true | `proj.owner == Main.myPlayer` | 生成、结算、特效 |
| 看别人的客户端 | `MultiplayerClient` | 别的玩家 false | 别人的射弹 false（`IsRemoteMirror()` 为 true） | 只表现，不生成 |
| 专用服务器 | `Server` | 全部 false | 全部 false | 转发 + 权威结算，不放特效 |

服务器端一律 false 是故意的：这样 `if (Projectile.IsOwnerSide())` 一行同时挡住“队友重复生成”和“服务器生成”。

## 2. 文件一览

| 文件 | 行数 | 作用 |
|---|---|---|
| `HJNetCode.cs` | 27 | 包总表 `Handler`、取包号 `PackHandleType<T>()`、总入口 `HandleHJPacket(reader, whoAmI)` |
| `BaseHJHandlePack.cs` | 29 | 所有包处理器的抽象基类，`Register()` 自动排队拿包号，子类只写 `Read()` |
| `HJNetAuthority.cs` | 51 | 权威判断：`IsOwnerSide` / `IsRemoteMirror` / `OwnerOrNull` / `OwnerAlive` / `CanSpawnChild` |
| `HJNetInput.cs` | 52 | 输入安全读取：本机 `Local*`、按键 `Action*/Skill*/Parry*`，代理 `AimWorldOf` / `MouseLeftOf` / `MouseRightOf` |
| `HJNetUtils.cs` | 60 | 发送端扩展：`player.SyncedMouseWorld(...)` 等四个 |
| `Content/ReadSyncMouseWorld.cs` | 33 | 准星坐标同步包 |
| `Content/ReadSyncMouseLeft.cs` / `ReadSyncMouseRight.cs` | 29 / 28 | 左右键按住状态同步包 |
| `Content/ReadWeaponSkill.cs` | 29 | 武器技能键同步包（发送端还没接，留着用） |
| `Content/ReadAutoSmelt.cs` | 36 | 自动烧矿包（原来写在 `Core/Packets/NetCode.cs` 里的那段，已搬进来） |
| `Globals/Players/NetPacket.cs` | 45 | 输入同步的发送方：只在变化时发包 |
| `HJScarletRework.cs` 的 `HandlePacket` | 1 行 | `HJNetCode.HandleHJPacket(reader, whoAmI);` |

## 3. 加一个新的同步包（三步）

假设要同步“充能等级”。

**第一步：建处理器类**，放在 `Core/NetCode/Content/` 下，不用在任何地方登记。

```csharp
public class ReadChargeLevel : BaseHJHandlePack
{
    public override void Read(BinaryReader reader, int whoAmI)
    {
        byte playerIndex = reader.ReadByte();
        byte level = reader.ReadByte();
        if (playerIndex < Main.maxPlayers && Main.player[playerIndex].active)
            Main.player[playerIndex].HJScarlet().ChargeLevel = level;
        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = HJScarletRework.Instance.GetPacket();
            packet.Write(Type);
            packet.Write(playerIndex);
            packet.Write(level);
            packet.Send(-1, whoAmI);
        }
    }
}
```

**第二步：发送**，和读取顺序逐项对齐（包头 int + 载荷）：

```csharp
if (Main.netMode == NetmodeID.MultiplayerClient)
{
    ModPacket pack = HJScarletRework.Instance.GetPacket();
    pack.Write(HJNetCode.PackHandleType<ReadChargeLevel>());
    pack.Write((byte)player.whoAmI);
    pack.Write((byte)level);
    pack.Send();
}
```

字段多了建议像 `HJNetUtils` 那样包成一个 `this Player` 扩展方法，调用处只看语义。

**第三步：字段自己加**，`ChargeLevel` 写在 `HJScarletPlayer` 的某个 partial 文件里；如果它会影响表现之外的数值结算，记得只让主人端或服务器去改。

要点：

- **先落地、再转发**，而且落地那段不要写进 `if (客户端)` 里。服务器也会执行落地，所以服务端的 `player.AimWorldOf()` 之类才有值——这正是旧的 `Core/Packets/NetCode.cs` 的毛病。
- `packet.Send()` 不带参数＝发给服务器；`packet.Send(-1, whoAmI)`＝广播并排除发起者；`packet.Send(toClient)`＝指定一个人。
- **包号是加载顺序决定的 int**。增删处理器类会让后面的编号整体平移，所以改了 `Content/` 之后客户端和服务器必须用同一份构建。混版本时 `HandleHJPacket` 会抛 `Received invalid packet index`，日志里一眼能看到，不会静默错读。

## 4. 权威判断的五种典型写法

### 4.1 挂载射弹：`HoldItem` 首行闸门

```csharp
public override void HoldItem(Item item, Player player)
{
    if (!player.IsOwnerSide())
        return;
    if (player.HasProj(Item.shoot))
        return;
    ...
}
```

现成的例子：`Items/Weapons/Executor/Firearm/ASMD.cs:46`、`Items/Weapons/Magic/Corona.cs:43`，一共 13 把武器已经这么写。
不写这一行的后果：每个客户端都给主人生成一份挂载射弹，射弹数量翻倍、特效翻倍。

### 4.2 射弹内部的特效与音效

```csharp
if (Projectile.IsOwnerSide())
    ScarletSound(HJScarletSounds.TheSevenStar_Swing, Projectile.Center, 0.75f, 1, ...);
```

例子：`Projs/Executor/StormSaberHeldProj.cs:47`、`Projs/Magic/CoronaHeldProj.cs`（爆炸粒子、火焰音效、烟雾都加了闸门）、`Projs/Executor/TheJudgementProj.cs`。

### 4.3 运行时生成子射弹 / 召唤形态：用 `CanSpawnChild()`

```csharp
if (!Projectile.CanSpawnChild())
    return;
```

`CanSpawnChild()` = 主人端 **且** 主人还活着。只写 `IsOwnerSide()` 会出这种事故：主人刚死，洁耳逻辑把召唤形态杀掉，而还在空中的父弹下一帧立刻又把它召唤回来。

例子：`Projs/Executor/DeathTollsProj.cs:81`、`Projs/Executor/ClimaticHawstringProj.cs:111`、`Projs/Executor/DreamlessNightProj.cs:211`、`Projs/Executor/DreamingLightProj.cs:158`，以及各召唤形态里发射激光/挂载星的地方。

### 4.4 绑定主人的射弹自己收尾（“召唤形态串到别人身上”的修法）

```csharp
public override void AI()
{
    Player boundOwner = Projectile.OwnerOrNull();
    if (boundOwner is null || boundOwner.dead)
    {
        Projectile.Kill();
        return;
    }
    ...
}
```

`Projectile.Kill()` 只作用于本地这一个射弹实例，所以每一端各杀各的那份镜像，不会留下“挂在别人槽位上”的召唤物。`OwnerOrNull()` 在槽位无效、玩家退出、槽位被回收时返回 `null`，这就是不串人的关键。

已经写好的：六把锤的 `*Minion`（`DeathTollsMinion` / `DreamlessNightMinion` / `EndlessWarMinion` / `TheJudgementMinion` / `DreamingLightMinion` / `ClimaticHawstringMinion`）和 `Projs/General/RuShiWoWenProj.cs`。

### 4.5 玩家级的定时生成：`PostUpdate` 会为**每个玩家**跑
    if (!Player.IsOwnerSide())
        return;
    ...
}
```

漏掉这一条时，每个客户端都会替别人的玩家生成一套随机召唤物。例子：`Globals/Players/PostUpdates.cs:163`。

## 5. 输入怎么读

| 需求 | 写法 | 备注 |
|---|---|---|
| 本机左/右键 | `HJNetInput.LocalMouseLeft` / `LocalMouseRight` | 服务器端为 false |
| 本机准星 | `HJNetInput.LocalAimWorld` | 服务器端为 `Vector2.Zero` |
| 通用动作键 / 技能键 / 格挡键 | `ActionJustPressed`、`ActionCurrent`、`ActionJustReleased`、`SkillJustPressed`、`ParryCurrent` | 内部已挡 `Main.dedServ` 并防空引用 |
| 某个玩家的瞄准点 | `player.AimWorldOf()` | 自己取本机，别人取同步值 |
| 某个玩家是否按左/右键 | `player.MouseLeftOf()` / `player.MouseRightOf()` | 同上 |

三条硬性要求：

1. **射弹/武器代码里不要出现裸的 `Main.MouseWorld`**。它是“看的人”的鼠标，队友端一用就会把别人的枪口朝向和后坐线拧到自己鼠标上。（`Projs/Executor/ASMDHeldProj.cs:156` 的后坐线那一行还没改，属于待清理项。）
2. **不要裸读 `HJScarletKeybinds.*Keybind.JustPressed/Current/JustReleased`**。专用服务器上键位从未初始化，会抛 `KeyNotFoundException`；一律走 `HJNetInput`。（仓库里还剩 18 处裸读，例如 `Globals/Methods/Executor.cs:140`。）
3. 队友端读到的输入是“变化时同步”的最新值，可能是上一帧的，够用；不要拿它做逐帧精确判定。要更精确就在关键帧额外发一个事件包。

数据来源：`Globals/Players/NetPacket.cs` 的 `UpdateNetPacket()`（在 `PostUpdate` 里调用），只在准星/左右键**发生变化**时发包，落地到 `SyncedMouseWorld` / `MouseLeft` / `MouseRight`。

## 6. 现有包速查

| 包类 | 载荷顺序 | 谁发 | 落在哪 | 怎么读 |
|---|---|---|---|---|
| `ReadSyncMouseWorld` | `byte` 玩家号 + `Vector2` | `NetPacket.UpdateNetPacket()`（变化时） | `HJScarletPlayer.SyncedMouseWorld` | `player.AimWorldOf()`、`player.ToMouseVector2()` |
| `ReadSyncMouseLeft` | `byte` + `bool` | 同上 | `MouseLeft` | `player.MouseLeftOf()` |
| `ReadSyncMouseRight` | `byte` + `bool` | 同上 | `MouseRight` | `player.MouseRightOf()` |
| `ReadWeaponSkill` | `byte` + `bool` | 暂无发送方 | `JustPressedWeaponSKill` | 预留，需要时调 `player.SyncedWeaponSkill(true)` |
| `ReadAutoSmelt` | 4 × `ushort`（**没有**玩家号字段） | `HJScarletGlobalTiles.PacketOres()` | `SmeltOres(x, y, chance, targetType)` | 不直接读 |

## 7. 常见坑

1. 写和读的顺序不一致：不报错，但数据错位。新加字段一律**追加在载荷末尾**。
2. 只在 `SetStaticDefaults` / 加载阶段之外发包，加载期发包没有对端。
3. 服务器分支写成“只转发不落地”：服务端的同步值会永远是零，任何依赖 `AimWorldOf()` 的服务器逻辑都错位。
4. 改了 `ai[]` 却没在同一帧设 `Projectile.netUpdate = true`：队友要等射弹自然同步才看到，表现为挂载射弹的后坐/阶段对不上。
5. 用 `Main.rand` 决定“要不要生成实体”：各端随机序列不同，会出现主人有子弹、队友没有。要生成就只在主人端生成，然后靠同步把状态带给别人。
6. 拿 `Projectile.owner` 直接索引 `Main.player[]` 而不判 `active`：槽位被回收时就变成“串到别人身上”。统一用 `OwnerOrNull()`。
7. 特效只在主人端跑，队友确实看不到那团粒子——这是有意的取舍（否则一份特效会乘以在场人数，还把不同步的随机值画出来）。要让队友看见，应该同步“事件”（`ai`、自定义同步字段、或一个事件包），而不是让队友端自己重新摇一遍。
8. 一次性事件（例如处决一击）目前不同步：`HJScarletGlobalProj.ExecutionStrike` 是本机 bool。想让队友端表现，需要按第 3 节加一个事件包。
9. `PacketHandleType<T>()` 在类没被加载时返回 `0`，会命中 0 号处理器；正常构建不会遇到，出现奇怪包内容先怀疑处理器没被 autoload（类名/命名空间/`public`/非抽象）。
10. 这套**没有**兜底机制：没有全局洁耳器、没有特效预算、没有玩家变量自动广播。忘写自查就是永久留在场上，忘写闸门就是每人一份。

## 8. 写完一把武器的自检清单

- [ ] `HoldItem` 第一行有 `if (!player.IsOwnerSide()) return;`
- [ ] 射弹内生成子射弹 / 召唤形态的地方用 `Projectile.CanSpawnChild()`
- [ ] 绑定主人的射弹在 `AI`/`ProjAI` 开头有 `OwnerOrNull()` + `dead` 自查
- [ ] 粒子、音效、屏幕震动只在主人端
- [ ] 输入全部走 `HJNetInput` / `*Of()`，代码里没有裸 `Main.MouseWorld` 和裸键位
- [ ] 关键 `ai[]` 改变的帧设了 `netUpdate`
- [ ] 单人、联机双开、专用服务器各跑一次，服务器日志里无异常、队友视角无重复实体

## 9. 这套刻意不做的事

- 不做“注册表 + 定时扫全场”的清理，改为**谁的射弹自己负责收尾**（第 4.4 节），逻辑读起来就在该出现的位置。
- 不做特效降配（按人数缩放数量/寿命），只有“跑 / 不跑”两档。
- 不做玩家变量的自动广播，需要同步就加一个小包类，代价一眼可见。
