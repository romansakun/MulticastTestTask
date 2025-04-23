# Тестовое задание
Прототип мобильной игры-головоломки с удалённой поставкой уровней (Unity Remote Config)

### Настройки

Assets/Content/Definitions - все JSON настройки игры:
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
* `Assets/Content/ScriptableSettings/ScriptableSettingsInstaller.asset` - scriptable настройки (цвета, звуки и прочее)

После изменений в JSON настройках их нужно собрать в единый файл и загрузить в Unity Remote Config:

* нажимаем кнопку _Definitions/Build_ в Unity:

![img](ReadMeAssets/definitions_build_hint.png)

* получаем файл: `Assets/Definitions.json`
* загружаем его в Unity Remote Config (ключ: _GameDefs_)

