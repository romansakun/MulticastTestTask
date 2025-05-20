# Form Words
### Настройки

`Assets/Content/Definitions` - все JSON настройки игры:
* `Assets/Content/Definitions/Levels/`
```json5
  {
      "Words":  //словарь
      {         // ключ - загаданное слово, значение - массив кластеров
        "FRIDGE" : [2, 2, 2],  // "FR", "ID", "GE"
        "GUITAR" : [3, 3],     // "GUI", "TAR"
        "HELMET" : [4, 2],     // "HELM", "ET"
        "INSECT" : [2, 4]      // "IN", "SECT"
      }
  }
```
* `Assets/Content/Definitions/Localizations/`
```json5
{
  "Levels": // словарь уровней для локализации
  {         // ключ - номер уровня, значение - его ID (название файла)
    "1": "En_1",  // 1 уровень тут: 'Assets/Content/Definitions/Levels/En_1.json'
    "2": "En_2",
    "3": "En_3",
    "4": "En_4"
  },
  "LocalizationText": // словарь локализации
  {
    "START": "Start!",
    //...
    "OK_BUTTON" : "Ok"
  }
}
```
* `Assets/Content/Definitions/DefaultSettings.json`
```json5
{
    "LocalizationDefId" : "Russian" //ID локализации по умолчанию
}
```

* `Assets/Content/Definitions/LevelSettings.json`
```json5
{
    "WordsRange" : { "Min" : 4, "Max" : 4 }, // диапазон кол-ва слов в уровнях
    "WordLengthsRange" : { "Min" : 6, "Max" : 6 }, // диапазон длины слов в уровнях
    "ClusterLengthsRange" : { "Min" : 2, "Max" : 4 }, // диапазон длины кластеров
    "RulesDescriptionLocalizationKey" : "GENERAL_LEVEL_RULES", // ключ локализации описания правил
    "LevelNumberLocalizationKey" : "LEVEL_NUMBER" // ключ локализации названия уровней ("Уровень {0}")
}
```

* `Assets/Content/ScriptableSettings/LocalGameDefs.json` - локальные настройки с 1 уровнем для каждой локализации
* `Assets/Content/ScriptableSettings/ScriptableSettingsInstaller.asset` - scriptable настройки. Содержит ссылку на локальный JSON, цвета, звуки и переменные.

После изменений в JSON настройках их нужно собрать в единый файл:

* нажимаем кнопку _Definitions/Build_ в Unity - это важно, т.к. все JSON валидируются при сборке:

![img](ReadMeAssets/definitions_build_hint.png)

* результат будет тут: `Assets/Content/ScriptableSettings/LocalGameDefs.json`

# Архитектура проекта

Сборки:
* `Assets/Scripts/GameLogic/`:
  - `Assets/Scripts/GameLogic/Model/` - загрузка/сохранение профиля игрока; получение JSON настроек и уровней. За основу взята идея паттерна репозиторий
  
  - `Assets/Scripts/GameLogic/Bootstrapper/` - логика загрузки (и перезагрузки) приложения. `Assets/Scripts/GameLogic/Bootstrapper/Loaders/UnityRemoteConfigLoader.cs` - загрузка настроек из Unity Remote Config 
  - `Assets/Scripts/GameLogic/UI/` - UI на основе MVVM подхода
* `Assets/Scripts/Infrastructure/` - инфраструктурные компоненты:
  - `Assets/Scripts/Infrastructure/LogicUtility/` - на основе паттерна цепочка обязанностей. Применим для сложной, разветвленной логики одной сущности (например ИИ бота). В данном случае применен для `Assets/Scripts/GameLogic/UI/Gameplay/ViewModel/GameplayViewModel.cs`
  - `Assets/Scripts/Infrastructure/Services/AssetLoader.cs` - загрузка ассетов через Addressables
