﻿namespace TeamProject; 
public class SceneManager {
    public Scene? CurrentScene { get; protected set; }
    public Scene? PrevScene { get; protected set; }

    private Dictionary<string, Scene> Scenes = new();
    private Dictionary<string, ActionOption> Options = new();

    public void Initialize() {
        // #1. 씬 정보 초기화.
        DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Scene");
        foreach (FileInfo info in directoryInfo.GetFiles()) {
            string sceneName = Path.GetFileNameWithoutExtension(info.FullName);
            Type? type = Type.GetType($"TeamProject.{sceneName}");
            if (type != null) {
                Scene? scene = Activator.CreateInstance(type) as Scene;
                Scenes.Add(sceneName, scene);
            }
        }

        // #2. 선택지 정보 초기화.
        Options.Add("NewGame", new("NewGame", "새로시작", () => EnterScene<CreateCharacterScene>()));
        Options.Add("LoadGame", new("LoadGame", "불러오기", LoadGame));
        Options.Add("Back", new("Back", "뒤로가기", () => EnterScene<Scene>(PrevScene.GetType().Name)));
        Options.Add("ShowInfo", new("ShowInfo", "상태보기", () => EnterScene<CharacterInfoScene>()));
        Options.Add("Inventory", new("Inventory", "인벤토리", () => EnterScene<InventoryInfoScene>()));
        Options.Add("Equipment", new("Equipment", "장비관리", () => EnterScene<EquipmentScene>()));
        Options.Add("Shop", new("Shop", "상점", () => EnterScene<ShopScene>()));
        Options.Add("Dungeon", new("Dungeon", "던전입구", () => EnterScene<DungeonGateScene>()));
        Options.Add("DungeonEnter", new("DungeonEnter", $"던전입장", () => EnterScene<BattleScene>()));
        Options.Add("Main", new("Main", "메인으로", () => EnterScene<MainScene>()));
        Options.Add("Rest", new("Rest", "휴식하기", () => EnterScene<RestScene>()));

        //
        Options.Add("UseInn", new("UseInn", "여관 이용하기", UseInn));
    }

    private void UseInn()
    {
        if (Game.Player.Hp == Game.Player.HpMax && Game.Player.Mp == Game.Player.MpMax)
            Renderer.Print(12, "지금 휴식할 필요는 없을 것 같다.", clear: true);

        else if (Game.Player.Gold <= 100)        
            Renderer.Print(12, "돈이 부족하다...", clear: true);
        
        else
        {
            Renderer.Print(12, "휴 식 중 . . . ", false, 2500, 2);
            Game.Player.Hp = Game.Player.HpMax;
            Game.Player.Mp = Game.Player.MpMax;
            Game.Player.ChangeGold(-100);
            Renderer.Print(5, $"당신의 체력 : {Game.Player.Hp} / {Game.Player.DefaultHpMax}");
            Renderer.Print(6, $"당신의 체력 : {Game.Player.Mp} / {Game.Player.DefaultMpMax}");
            Renderer.Print(8, $"보유 골드 : {Game.Player.Gold} G");
            Renderer.Print(12, "휴식을 마치니, 당신의 의지가 차오른다.", clear: true);
        }            
    }

    #region ActionOption

    public ActionOption GetOption(string key) => Options[key];

    #endregion

    #region Scene

    /// <summary>
    /// 씬을 불러옵니다.
    /// </summary>
    /// <typeparam name="T">씬 타입을 결정합니다.</typeparam>
    /// <param name="sceneKey"></param>
    /// <returns></returns>
    public Scene GetScene<T>(string? sceneKey = null) where T : Scene {
        if (string.IsNullOrEmpty(sceneKey)) sceneKey = typeof(T).Name;
        if (!Scenes.TryGetValue(sceneKey, out Scene? scene)) return null;
        return scene;
    }

    /// <summary>
    /// 씬에 진입합니다.
    /// </summary>
    /// <typeparam name="T">씬 타입을 결정합니다.</typeparam>
    /// <param name="sceneKey"></param>
    public void EnterScene<T>(string? sceneKey = null) where T : Scene {
        // #1. Scene 불러오기.
        if (string.IsNullOrEmpty(sceneKey)) sceneKey = typeof(T).Name;
        if (!Scenes.TryGetValue(sceneKey, out Scene? scene)) return;
        if (scene == null || scene == CurrentScene) return;

        // #2. 이전 씬 설정.
        SetPrevScene();

        // #3. 현재 씬 진입.
        CurrentScene = scene;
        scene.EnterScene();
        scene.NextScene();
    }


    private void SetPrevScene() {
        PrevScene = CurrentScene;
    }

    #endregion

    private void LoadGame() {
        Game.Player = Managers.Game.data.character;
        Game.Stage = Managers.Game.data.stage;
        EnterScene<MainScene>();
    }
}

public class ActionOption {
    public string Key { get; private set; }
    public string Description { get; private set; }
    public Action Action { get; private set; }
    public ActionOption(string key, string description, Action action) {
        Key = key;
        Description = description;
        Action = action;
    }

    public void Execute() => Action?.Invoke();
}

public enum Command
{
    Nothing,
    MoveTop,
    MoveBottom,
    MoveLeft,
    MoveRight,
    Interact,
    Exit,
}

public enum BattleAction
{
    SelectAction,
    SelectSkill,
    SelectAttack,
    Attack,
    UsePotion
}

