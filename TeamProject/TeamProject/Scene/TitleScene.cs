﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject {
    public class TitleScene : Scene {
        public override void EnterScene() {
            Options.Clear();
            Options.Add(Managers.Scene.GetOption("NewGame"));
            Options.Add(Managers.Scene.GetOption("LoadGame"));

            DrawScene();
        }

        public override void NextScene() {
            do {
                Renderer.PrintOptions(7, Options, true, selectedOptionIndex);
                GetInput();
            }
            while (lastCommand != Command.Interact);
        }

        protected override void DrawScene() {
            Renderer.DrawBorder();
            Renderer.Print(5, "와 재미잇는 깨임");
            Renderer.PrintOptions(7, Options, true, selectedOptionIndex);
            Renderer.PrintKeyGuide("[방향키 ↑ ↓: 선택지 이동] [Enter: 선택]");
        }
    }
}
