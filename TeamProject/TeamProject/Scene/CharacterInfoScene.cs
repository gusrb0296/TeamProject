﻿namespace TeamProject;

public class CharacterInfoScene : Scene 
{
    public override string Title { get; protected set; } = "상 태 보 기";
    public override void EnterScene() 
    {
        Options.Clear();
        Options.Add(Managers.Scene.GetOption("Back"));
        DrawScene();
    }

    public override void NextScene()
    {
        do {
            GetInput();
        } while (Managers.Scene.CurrentScene is CharacterInfoScene);
    }
    protected override void DrawScene() 
    {
        Renderer.DrawBorder(Title);
        Renderer.Print(3, "캐릭터의 정보가 표시됩니다.");

        // ==== 캐릭터 정보 표시 ====
        Renderer.Print(5, $"Lv. {Game.Player.Level}");
        Renderer.Print(6, $"{Game.Player.Name} ( {Game.Player.Job} )");
        Renderer.Print(7, $"공격력 : {Game.Player.DefaultDamage}");
        Renderer.Print(8, $"방어력 : {Game.Player.DefaultDefense}");
        Renderer.Print(9, $"체  력 : {Game.Player.Hp} / {Game.Player.DefaultHpMax}");
        Renderer.Print(10, $"마  나 : {Game.Player.Mp} / {Game.Player.DefaultMpMax}");
        Renderer.Print(10, $"경험치 : {Game.Player.TotalExp} / {Game.Player.NextLevelExp}");
        Renderer.Print(11, $"치명타 :{Game.Player.Critical * 100:00}%");
        Renderer.Print(12, $"회피율 :{Game.Player.Avoid * 100:00}%");
        Renderer.Print(13, $"Gold : {Game.Player.Gold} G");

        Renderer.PrintKeyGuide("[ESC : 뒤로가기]");
    }
}