using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class ShowHideButton : MonoBehaviour
{
    public GlitchUITransition[] show;
    public GlitchUITransition[] hide;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Execute);
    }

    private void Execute()
    {
        foreach (var item in hide)
        {
            item.element.Hide(true, item.transition);
        }

        foreach (var item in show)
        {
            item.element.Show(true, item.transition);
        }
    }
}
