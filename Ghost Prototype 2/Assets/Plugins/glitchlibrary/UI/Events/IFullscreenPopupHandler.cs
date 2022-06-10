using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.EventSystems
{
    public interface IFullscreenPopupHandler : IEventSystemHandler
    {
        // this exists so that buttons with a release event can get the release and deal with it correctly!
        void OnPopupShow();
        void OnPopupHide();
    }
}