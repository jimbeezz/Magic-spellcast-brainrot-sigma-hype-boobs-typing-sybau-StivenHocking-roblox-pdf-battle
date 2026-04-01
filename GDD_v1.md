# Game Design Doc v1

Проект: `SpellCaster: Arena`  
Версия документа: v1 (first playable target)

## 1. Цель first playable

Собрать вертикальный срез, в котором игрок:

- понимает core-loop за 2-3 минуты;
- проходит цепочку из 6 боев (включая 2 босса и финального дракона);
- получает ощутимый рост силы через unlock заклинаний;
- заходит в режим `Exam` как в отдельный челлендж.

## 2. Core-loop

1. Выбор боя/режима.
2. Предбой: выбор loadout из выученных заклинаний (4 активных слота).
3. В бою: чтение паттерна из панели известных спеллов -> ввод -> каст -> реакция врага.
4. Победа -> награда XP -> покупка улучшений.
5. Переход к следующему, более сложному противнику.

## 3. Формат данных (референс)

```ts
type Spell = {
  id: string;
  displayName: string;
  tier: 1 | 2 | 3 | 4;
  school: "fire" | "ice" | "lightning" | "psy" | "ward" | "ancient";
  inputPattern: string;
  patternDifficulty: number; // 1..10
  baseDamage: number;
  manaCost: number;
  castWindowMs: number;
  effects?: Array<"freeze" | "slow" | "interrupt" | "shieldBreak" | "scan" | "cleanse">;
};

type PlayerLoadout = {
  selectedSpellIds: string[]; // ровно 4 в Arena
};
```

## 3.1 Правило доступности заклинаний

- Игрок не обязан запоминать паттерны наизусть.
- В бою всегда видна панель выученных заклинаний:
  - название,
  - паттерн,
  - стоимость маны,
  - кулдаун/статус недоступности (если применимо).
- Каст можно инициировать только заклинаниями из текущего loadout.

## 4. Стартовые заклинания (15 шт.)

### 4.1 Tier 1 (доступны сразу)

| id | Название | Школа | Паттерн | Dmg | Mana | Window (ms) | Эффект |
|---|---|---|---|---:|---:|---:|---|
| sp_fire_01 | Ember Tap | fire | `Fb8alL` | 12 | 8 | 2600 | - |
| sp_ice_01 | Cold Pin | ice | `icE#2` | 10 | 7 | 2700 | slow(20%, 2s) |
| sp_lgt_01 | Spark Cut | lightning | `zAp!7` | 14 | 10 | 2400 | - |
| sp_ward_01 | Quick Ward | ward | `w@rd` | 0 | 6 | 2200 | shield(30%, 1 hit) |

### 4.2 Tier 2 (unlock после 5 побед)

| id | Название | Школа | Паттерн | Dmg | Mana | Window (ms) | Эффект |
|---|---|---|---|---:|---:|---:|---|
| sp_fire_02 | Flame Arc | fire | `fL@me/9` | 20 | 14 | 2300 | burn(4/s, 3s) |
| sp_ice_02 | Frost Lock | ice | `fr0st*К` | 18 | 13 | 2350 | freeze(0.7s) |
| sp_lgt_02 | Chain Snap | lightning | `ch^ain//3` | 22 | 16 | 2200 | chain(2 цели) |
| sp_psy_01 | Mind Needle | psy | `пси!N3` | 16 | 11 | 2400 | ignoresChaos(1 cast) |

### 4.3 Tier 3 (unlock после 10 побед)

| id | Название | Школа | Паттерн | Dmg | Mana | Window (ms) | Эффект |
|---|---|---|---|---:|---:|---:|---|
| sp_fire_03 | Pyro Lance | fire | `пр@ок^az/*za` | 30 | 20 | 2000 | shieldBreak(40%) |
| sp_ice_03 | Blizzard Coil | ice | `Хл0д=>[]` | 27 | 19 | 2050 | slow(45%, 3s) |
| sp_lgt_03 | Volt Rift | lightning | `vOLT~->7!` | 33 | 23 | 1900 | interrupt |
| sp_ward_02 | Mirror Ward | ward | `[]W@rD[]` | 0 | 16 | 1900 | reflect(40%, 1 hit) |

### 4.4 Tier 4 (unlock после 20 побед)

| id | Название | Школа | Паттерн | Dmg | Mana | Window (ms) | Эффект |
|---|---|---|---|---:|---:|---:|---|
| sp_anc_01 | Relic Pulse | ancient | `☻↓→♫kZ` | 46 | 28 | 1800 | bossBonus(1.35x) |
| sp_anc_02 | Null Script | ancient | `☼x_Я>☻<7` | 52 | 32 | 1700 | stripResist(1 turn) |
| sp_anc_03 | Last Verdict | ancient | `♫☻↓~A9/Ж` | 60 | 38 | 1600 | execute(+25% <30% hp) |

## 5. Противники (6 шт.)

## 5.1 Goblin Pyromaniac

- HP: 55
- Damage: 8-12 every 2.6s
- Vulnerability: ice x2.0
- Resistance: fire x0.6
- Trait: каждый 3-й каст `Fire Burst` (22 dmg), можно прервать только `interrupt`.
- Награда: 40 XP

## 5.2 Ice Golem

- HP: 110
- Damage: 6-9 every 3.0s
- Vulnerability: fire x1.6
- Resistance: ice x0.3
- Trait: каждые 2 действия ставит `Stone Shield` (-50% входящего урона на 1 ход).
- Награда: 55 XP

## 5.3 Electric Elemental

- HP: 48
- Damage: 14-20 every 2.1s
- Vulnerability: psy x1.4
- Resistance: lightning immunity (x0)
- Trait: после получения урона уменьшает окно следующего каста игрока на 300ms.
- Награда: 60 XP

## 5.4 Dark Magus (boss)

- HP: 150
- Damage: 11-16 every 2.4s
- Vulnerability: меняется каждые 8s, видна после `scan`/аналога.
- Resistance: циклическая к 2 школам (по 0.5).
- Trait: копирует последнее успешное заклинание игрока (кроме ancient/ward).
- Награда: 120 XP

## 5.5 Chaotic Magus (boss)

- HP: 120
- Damage: 6-24 every 2.3s
- Vulnerability: случайная каждые 6s
- Resistance: случайная каждые 6s
- Trait: каждые 10s включает `Reality Twist`:
  - reverse input или
  - Caesar +1 shift или
  - swapCase rule.
- `psy` игнорирует искажение на 1 каст.
- Награда: 130 XP

## 5.6 Ancient Dragon (final boss)

- HP: 260
- Damage: 20-30 every 2.0s
- Vulnerability: ancient x1.75
- Resistance: all non-ancient x0.25
- Trait: каждые 3 атаки использует `Inferno Breath` (42 dmg).
- Контрмеханика: надо ввести `Quick Ward` или `Mirror Ward` в окно 2000ms, иначе полный урон.
- Награда: 240 XP

## 6. Баланс: формулы v1

## 6.1 Урон заклинания

`finalDamage = round(baseDamage * schoolMod * critMod * tempoMod * enemyStateMod)`

Где:

- `schoolMod`: из таблицы уязвимостей/резистов (например 2.0, 1.0, 0.6, 0.25, 0).
- `critMod`:
  - 1.25, если каст завершен за <= 40% окна;
  - иначе 1.0.
- `tempoMod`:
  - 1.0 + min(0.15, combo * 0.02), где `combo` = успешные касты подряд в бою.
- `enemyStateMod`: щит, фаза, дебаффы (обычно 0.5-1.35).

## 6.2 Точность и штрафы

- Ошибка в паттерне:
  - Arena: каст провален, -15% текущей combo-надбавки;
  - Exam normal: combo = 0;
  - Exam strict: конец попытки.

## 6.3 Восстановление маны

- Пассивно: +4 mana / сек.
- За идеальный каст (без задержки и ошибок): +3 mana.
- Hard floor: mana не уходит ниже 0.

## 6.4 Рекомендованные стартовые статы игрока

- HP: 120
- Mana: 100
- Base cast grace: +0ms (без бонуса)
- Базовая ошибка: 0 (без tolerance)

## 7. Прогрессия и экономика

## 7.1 XP и unlock

- Tier 2: 5 побед или 220 XP
- Tier 3: 10 побед или 520 XP
- Tier 4: 20 побед или 1100 XP

## 7.2 Улучшения (покупки)

- `Cast Focus I`: +120ms к окну каста (цена 140 XP)
- `Cast Focus II`: +120ms к окну каста (цена 260 XP)
- `Mana Vessel I`: +20 max mana (цена 180 XP)
- `Vital Core I`: +25 max HP (цена 180 XP)
- `Error Guard` (опция): 1 опечатка игнорируется раз в 20s (цена 320 XP)

## 8. Exam mode scoring v1

`score = basePoints + speedBonus + streakBonus + difficultyBonus`

- `basePoints = successfulCasts * 100`
- `speedBonus = sum(max(0, 35 - castTimePercent))`
- `streakBonus = maxCombo * 25`
- `difficultyBonus = successfulHardPatterns * 40`

Поля итогового экрана:

- score
- successfulCasts
- maxCombo
- accuracy (%)
- castsPerMinute
- bestPatternTier

## 9. UI минимальный состав (first playable)

- Центр: активный паттерн и посимвольный прогресс.
- Верх: HP/mana игрока, HP врага.
- Справа: таймер следующей атаки врага.
- Слева: панель выученных заклинаний (паттерны всегда отображаются).
- Снизу: быстрый лог событий (interrupt, shield, resist, crit).
- Для Exam: большой таймер сессии + текущая серия.
- Перед боем: экран выбора 4 заклинаний в loadout.

## 10. Тест-критерии first playable

- Игрок может пройти 3 обычных врага подряд без софтлока.
- Боссы используют свои особые механики минимум 2 раза за бой.
- Dragon-энкаунтер проходим при корректном использовании ward + ancient.
- Exam стабильно считает score и combo на 100 последовательных кастов.
- Нет пропусков ввода при быстрых последовательностях.
