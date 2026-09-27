using UnityEngine;

public enum TitleUIState { Hidden, Showing, Visible, Hiding }

// Model: initial geometry and transition state.
public sealed class TitleUIModel
{
    public Vector2 ButtonsVisible { get; }
    public Vector2 TitleVisible { get; }
    public Vector2 ButtonsHidden { get; }
    public Vector2 TitleHidden { get; }
    public TitleUIState State { get; set; } = TitleUIState.Hidden;

    public TitleUIModel(Vector2 buttonsVisible, Vector2 titleVisible,
        Vector2 buttonsHidden, Vector2 titleHidden)
    {
        ButtonsVisible = buttonsVisible;
        TitleVisible = titleVisible;
        ButtonsHidden = buttonsHidden;
        TitleHidden = titleHidden;
    }
}
