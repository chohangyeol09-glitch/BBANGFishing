using UnityEngine;

// Controller: button actions and transitions.
public sealed class TitleUIController
{
    private readonly TitleUI _view;
    private readonly TitleUIModel _model;

    public TitleUIController(TitleUI view, TitleUIModel model)
    {
        _view = view;
        _model = model;
    }

    public void Show()
    {
        _view.StopAnimation();
        _view.SetInteractable(false);
        _view.SetPositions(_model.ButtonsHidden, _model.TitleHidden);
        _model.State = TitleUIState.Showing;
        _view.Animate(_model.ButtonsVisible, _model.TitleVisible, true, () =>
        {
            _model.State = TitleUIState.Visible;
            _view.SetInteractable(true);
        });
    }

    public void StartGame()
    {
        if (_model.State != TitleUIState.Visible) return;
        _model.State = TitleUIState.Hiding;
        _view.SetInteractable(false);
        _view.Animate(_model.ButtonsHidden, _model.TitleHidden, false, () =>
        {
            _model.State = TitleUIState.Hidden;
            _view.NotifyStartCompleted();
        });
    }

    public void OpenSettings()
    {
        if (_model.State == TitleUIState.Visible) _view.NotifySettingsRequested();
    }

    public void Quit()
    {
        if (_model.State == TitleUIState.Visible) Application.Quit();
    }

    public void Suspend()
    {
        _view.StopAnimation();
        _view.SetInteractable(false);
        _view.SetPositions(_model.ButtonsVisible, _model.TitleVisible);
        _model.State = TitleUIState.Hidden;
    }
}
