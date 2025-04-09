using System;
using System.Collections.Generic;

namespace TopDownShooter
{
    public static class UI
    {
        private static Dictionary<LayoutIndex, UILayout> layouts;
        private static LayoutIndex activeLayout = LayoutIndex.MainMenu;
        public static LayoutIndex ActiveLayout { get { return activeLayout; } }

        public static void Initialize()
        {
            layouts = new()
            {
                [LayoutIndex.MainMenu] = new MainMenu(),
                [LayoutIndex.LevelSelect] = new LevelSelect(),
                [LayoutIndex.PauseMenu] = new PauseMenu()
            };
        }

        public static void Draw()
        {
            foreach (var kvp in layouts)
            {
                if (kvp.Key == activeLayout)
                {
                    kvp.Value.Draw();
                }
            }
        }
        public static void Update()
        {
            foreach (var kvp in layouts)
            {
                if (kvp.Key == activeLayout)
                {
                    kvp.Value.Update();
                }
            }
        }
        public static void SwitchToMainMenu(object sender, EventArgs e)
        {
            activeLayout = LayoutIndex.MainMenu;
        }
        public static void SwitchToLevelSelect(object sender, EventArgs e)
        {
            activeLayout = LayoutIndex.LevelSelect;
        }
        public static void SwitchToHUD(object sender, EventArgs e)
        {
            activeLayout = LayoutIndex.HUD;
        }
        public static void SwitchToPauseMenu(object sender, EventArgs e)
        {
            activeLayout = LayoutIndex.PauseMenu;
            World.Pause();
        }
        public static void StartLevel1(object sender, EventArgs e)
        {
            World.ChangeLevel(1);
            SwitchToHUD(sender, e);
        }
        public static void StartLevel2(object sender, EventArgs e)
        {
            World.ChangeLevel(2);
            SwitchToHUD(sender, e);
        }
        public static void ResumeButton(object sender, EventArgs e)
        {
            SwitchToHUD(sender, e);
            World.Resume();
        }
        public static void BackToMenuButton(object sender, EventArgs e)
        {
            SwitchToMainMenu(sender, e);
            World.Reset();
        }
    }
    public enum LayoutIndex
    {
        MainMenu = 0,
        LevelSelect = 1,
        HUD = 2,
        PauseMenu = 3,
    }
    public abstract class UILayout : UIComponent
    {
        protected List<Container> children = new List<Container>();
        public void Draw()
        {
            foreach (Container child in children)
            {
                child.Draw();
            }
        }
        public void Update()
        {
            foreach (Container child in children)
            {
                child.Update();
            }
        }
    }
    internal sealed class MainMenu : UILayout
    {
        public MainMenu()
        {
            Vector2 playPos = new Vector2(789, 511);
            Vector2 exitPos = new Vector2(834, 707);
            Vector2 logoPos = new Vector2(729, 98);

            Button playButton = new Button(playPos, 1f, Globals.Content.Load<Texture2D>("Play"), Globals.Content.Load<Texture2D>("PlayHover"));
            Button exitButton = new Button(exitPos, 1f, Globals.Content.Load<Texture2D>("Exit"), Globals.Content.Load<Texture2D>("ExitHover"));
            Image logo = new Image(logoPos, 1f, Globals.Content.Load<Texture2D>("Title"));

            playButton.Click += UI.SwitchToLevelSelect;
            exitButton.Click += Main.instance.ExitWrapper;

            Container container = new Container(new Vector2(0,0), 1f, playButton, exitButton, logo);
            children.Add(container);
        }
    }
    internal sealed class LevelSelect : UILayout
    {
        public LevelSelect()
        {
            Vector2 logoPos = new Vector2(729, 98);
            Vector2 level1Pos = new Vector2(654, 350);
            Vector2 level2Pos = new Vector2(766, 350);
            //Vector2 level3Pos = new Vector2(878, 350);
            //Vector2 level4Pos = new Vector2(990, 350);


            Image logo = new Image(logoPos, 1f, Globals.Content.Load<Texture2D>("Title"));
            Button level1 = new Button(level1Pos, 1f, Globals.Content.Load<Texture2D>("Level1Button"), Globals.Content.Load<Texture2D>("Level1ButtonHover"));
            Button level2 = new Button(level2Pos, 1f, Globals.Content.Load<Texture2D>("Level2Button"), Globals.Content.Load<Texture2D>("Level2ButtonHover"));
            //Button level3 = new Button(level3Pos, 1f, Globals.Content.Load<Texture2D>("Level3Button"), Globals.Content.Load<Texture2D>("Level3ButtonHover"));
            //Button level4 = new Button(level4Pos, 1f, Globals.Content.Load<Texture2D>("Level4Button"), Globals.Content.Load<Texture2D>("Level4ButtonHover"));

            level1.Click += UI.StartLevel1;
            level2.Click += UI.StartLevel2;

            Container container = new Container(new Vector2(0, 0), 1f, logo, level1, level2);
            children.Add(container);
        }
    }
    internal sealed class PauseMenu : UILayout
    {
        public PauseMenu()
        {
            Vector2 resumePos = new Vector2(711, 513);
            Vector2 backToMenuPos = new Vector2(734, 692);
            Vector2 logoPos = new Vector2(681, 147);

            Button resumeButton = new Button(resumePos, 1f, Globals.Content.Load<Texture2D>("Resume"), Globals.Content.Load<Texture2D>("ResumeHover"));
            Button backToMenuButton = new Button(backToMenuPos, 1f, Globals.Content.Load<Texture2D>("BackToMenu"), Globals.Content.Load<Texture2D>("BackToMenuHover"));
            Image logo = new Image(logoPos, 1f, Globals.Content.Load<Texture2D>("Pause"));

            resumeButton.Click += UI.ResumeButton;
            backToMenuButton.Click += UI.BackToMenuButton;

            Container container = new Container(new Vector2(0, 0), 1f, resumeButton, backToMenuButton, logo);
            children.Add(container);
        }
    }
    public abstract class UIComponent
    {
        public virtual Vector2 Position { get; }
        protected Vector2 position;

        protected Rectangle bounds;

        protected float scale;

        public virtual void MoveBy(Vector2 offset)
        {
            position += offset;
        }
        public virtual void SetPosition(Vector2 position)
        {
            this.position = position;
        }
    }
    public abstract class UIElement : UIComponent
    {
        public UIElement(Vector2 position, float scale)
        {
            this.position = position;
            this.scale = scale;
        }
        public abstract void Update();
        public abstract void Draw();
        public virtual bool IsMousedOver()
        {
            if (bounds.Contains(World.MouseScreen))
            {
                return true;
            }
            return false;
        }
    }
    public class Button : UIElement
    {
        private Texture2D imageTexture;
        private Texture2D hoverTexture;

        public event EventHandler Click;
        public Button(Vector2 position, float scale, Texture2D imageTexture, Texture2D hoverTexture) : base(position, scale)
        {
            this.imageTexture = imageTexture;
            this.hoverTexture = hoverTexture;
            this.bounds = imageTexture.Bounds;
            this.bounds.Location = position.ToPoint();
        }
        private void OnClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }
        public override void Draw()
        {
            if (IsMousedOver())
            {
                Globals.SpriteBatch.Draw(hoverTexture, position, Color.White);
            }
            else
            {
                Globals.SpriteBatch.Draw(imageTexture, position, Color.White);
            }
        }

        public override void Update()
        {
            if (IsMousedOver())
            {
                if (Input.MouseLeftJustPressed)
                {
                    OnClick(EventArgs.Empty);
                }
            }
        }
    }
    public class Image : UIElement
    {
        private Texture2D imageTexture;

        public Image(Vector2 position, float scale, Texture2D imageTexture) : base(position, scale)
        {
            this.imageTexture = imageTexture;
            this.bounds = imageTexture.Bounds;
            this.bounds.Location = position.ToPoint();
        }
        public override void Update()
        {
        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(imageTexture, position, Color.White);
        }
    }
    public class Container : UIElement
    {
        private List<UIElement> children = new();
        public Container(Vector2 position, float scale,params UIElement[] children) : base(position, scale)
        {
            foreach (var element in children)
            {
                this.children.Add(element);
            }
        }

        public override void Update()
        {
            foreach (var child in children)
            { 
                child.Update();
            }
        }
        public override void Draw()
        {
            foreach (var child in children)
            {
                child.Draw();
            }
        }
        public void RegisterElement(params UIElement[] elements)
        {
            foreach (var element in elements)
            { 
                children.Add(element);
            }
        }
        public override void MoveBy(Vector2 offset)
        {
            position += offset;
            foreach (var child in children)
            {
                child.MoveBy(offset);
            }
        }
        public new void SetPosition(Vector2 position)
        {
            this.position = position;
            foreach (var child in children)
            {
                child.SetPosition(position);
            }
        }
    }
}
