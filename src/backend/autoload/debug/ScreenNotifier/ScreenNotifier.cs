using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rubicon.backend.common.enums;

namespace Rubicon.backend.autoload.debug.ScreenNotifier;

[Icon("res://assets/miscicons/autoload.png")]
public partial class ScreenNotifier : CanvasLayer
{
    private Panel NotificationInstance;
    private Queue<(Panel, double)> notificationQueue = new();
    private float YOffset;

    [Export] private Color InfoNotificationColor { get; set; } = new(0.32f,0.32f, 0.32f);
    [Export] private Color WarnNotificationColor { get; set; } = new(0.79f,0.79f, 0);
    [Export] private Color ErrorNotificationColor { get; set; } = new(0.70f, 0f, 0f);
    
    public static ScreenNotifier Instance { get; private set; }
    
    public override void _EnterTree() => Instance = this;
    public override void _Ready() => NotificationInstance = GetNode<Panel>("Notification");
    public override void _Process(double delta) => UpdateProgressBar(delta);
    
    public void Notify(string message, bool printToConsole = true, NotificationType type = NotificationType.Info, float duration = 5.0f)
    {
        StackTrace stackTrace = new();
        StackFrame stackFrame = stackTrace.GetFrame(1);
        
        string fullMessage = $"[{type.ToString().ToUpper()} - {stackFrame!.GetMethod()?.Name}] -> {message}";
        
        if (NotificationInstance.Duplicate() is Panel notificationInstance)
        {
            if (YOffset == 0) YOffset = 32;
            else YOffset += notificationInstance.GetRect().Size.Y + 10;

            notificationInstance.Visible = true;
            notificationInstance.Position = new(notificationInstance.GetRect().Position.X, YOffset);

            var progressBar = notificationInstance.GetNode<ProgressBar>("DurationBar");
            var messageLabel = notificationInstance.GetNode<Label>("Message");
            var animalationtolongplayer = notificationInstance.GetNode<AnimationPlayer>("animalationtolongplayer");

            switch (type)
            {
                case NotificationType.Info:
                    if (printToConsole) GD.Print(fullMessage);
                    break;
                case NotificationType.Warning:
                    if (printToConsole) GD.Print(fullMessage);
                    break;
                case NotificationType.Error:
                    if (printToConsole) GD.PrintErr(fullMessage);
                    break;
            }

            messageLabel.Text = fullMessage;
            progressBar.MaxValue = duration;
            progressBar.Value = duration;
            animalationtolongplayer.Play("in");

            messageLabel.MinimumSizeChanged += MessageLabelOnMinimumSizeChanged;
            void MessageLabelOnMinimumSizeChanged()
            {
                messageLabel.MinimumSizeChanged -= MessageLabelOnMinimumSizeChanged;
                notificationInstance.Size = new(messageLabel.Size.X + 20, messageLabel.Size.Y);
                progressBar.Size = new(messageLabel.Size.X + 20, progressBar.Size.Y);
                messageLabel.Position = new((notificationInstance.Size.X - messageLabel.Size.X) / 2, (notificationInstance.Size.Y - messageLabel.Size.Y) / 2);
            }
            
            notificationQueue.Enqueue((notificationInstance, duration));
            Instance.AddChild(notificationInstance);
        }
    }
    
    private void UpdateProgressBar(double delta)
    {
        for (int i = 0; i < notificationQueue.Count; i++)
        {
            var (panel, duration) = notificationQueue.Dequeue();
            duration -= delta;
            var progressBar = panel.GetNode<ProgressBar>("DurationBar");
            var animalationtolongplayer = panel.GetNode<AnimationPlayer>("animalationtolongplayer");
            progressBar.Value = duration;

            if (Mathf.Abs(duration - 0.5f) < 0.01f) animalationtolongplayer.Play("out");
            if (duration <= 0) OnNotificationTimeout(panel);
            else notificationQueue.Enqueue((panel, duration));
        }
    }

    private void OnNotificationTimeout(Panel panel)
    { 
        notificationQueue = new(notificationQueue.Where(item => item.Item1 != panel));
        panel.QueueFree();
        YOffset -= panel.GetRect().Size.Y + 10;
    
        float yOffset = YOffset;
        foreach (var (remainingPanel, _) in notificationQueue)
        {
            remainingPanel.Position = new(remainingPanel.Position.X, yOffset);
            yOffset += remainingPanel.GetRect().Size.Y + 10;
        }
    }
}
