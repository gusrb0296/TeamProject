﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject;
public class RestScene : Scene {
    public override string Title { get; protected set; } = "토리엘의 집";

    public override void EnterScene()
    {
        MusicPlayer.Instance.music = "Home.mp3";
        MusicPlayer.Instance.PlayAsync(0.05f); // 음악파일명, 볼륨
        // #1. 선택지 설정.
        Options.Clear();
        Options.Add(Managers.Scene.GetOption("UseInn"));
        Options.Add(Managers.Scene.GetOption("Back"));
        DrawScene();
    }

    public override void NextScene()
    {
        do
        {
            Renderer.PrintOptions(15, Options, true, selectedOptionIndex);
            GetInput();
        }
        while (Managers.Scene.CurrentScene is RestScene);
    }
    protected override void DrawScene()
    {
        Renderer.DrawBorder(Title);
        Renderer.Print(3, "집에서 휴식하시겠습니까?");
                
        Renderer.Print(5, $"당신의 체력 : {Game.Player.Hp} / {Game.Player.DefaultHpMax}");
        Renderer.Print(6, $"당신의 마나 : {Game.Player.Mp} / {Game.Player.DefaultMpMax}");
        Renderer.Print(8, $"보유 골드 : {Game.Player.Gold} G");
        Renderer.Print(9, "이용 골드 : 100 G");

        Renderer.PrintKeyGuide("[ESC : 뒤로가기]");
    }    
}

