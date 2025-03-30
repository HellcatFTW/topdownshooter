using System;
using System.Collections.Generic;

namespace TopDownShooter
{

    public static class UI
    {
        private static List<UILayout> layouts = new List<UILayout>();

        public static void Initialize()
        {
            layouts.Add(new MainMenu());
        }

        public static void Draw()
        {
            foreach (UILayout layout in layouts)
            {
                if (!layout.IsActive)
                {
                    continue;
                }
                layout.Draw();
            }
        }
        public static void Update()
        {
            foreach (UILayout layout in layouts)
            {
                if (!layout.IsActive)
                {
                    continue;
                }
                layout.Update();
            }
        }
    }
    public abstract class UILayout : UIComponent
    {
        protected List<Container> children = new List<Container>();
        public bool IsActive { get; set; } = false;
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

            Button playButton = new Button(playPos, 1f, null, Globals.Content.Load<Texture2D>("Play"));
            Button exitButton = new Button(exitPos, 1f, null, Globals.Content.Load<Texture2D>("Exit"));
            Image logo = new Image(logoPos, 1f, Globals.Content.Load<Texture2D>("Title"));

            Container container = new Container(new Vector2(0,0), 1f, playButton, exitButton, logo);
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
        private String text;
        private Texture2D imageTexture;

        public event EventHandler Click;
        public Button(Vector2 position, float scale,string text, Texture2D imageTexture) : base(position, scale)
        {
            this.text = text;
            this.imageTexture = imageTexture;
            this.bounds = imageTexture.Bounds;
        }
        private void OnClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }
        public override void Draw()
        {
            Globals.SpriteBatch.Draw(imageTexture, position, Color.White);
        }

        public override void Update()
        {
            if (IsMousedOver() && Input.MouseLeft)
            {
                OnClick(EventArgs.Empty);
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
