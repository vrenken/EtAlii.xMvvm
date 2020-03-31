namespace EtAlii.xMvvm
{
    using UnityEngine;

    public class ViewSystem : MonoBehaviour
    {
        [SerializeField]
        public GameObject loginPanel;
        public LoginView loginView;

        public GameObject compositionPanel;
        public CompositionView compositionView;

        public GameObject locationsGlobe;
        public LocationsView locationsView;

         private void Start()
         {
             // This is just a simple method to wire up the View with ViewModels.
             // There are most probably better ways to do so.  
             loginView = new LoginView(loginPanel)
             {
                 ViewModel = new LoginViewModel
                 {
                     UserName = "john.doe@nomail.com",
                     Password = "1234"
                 }
             };
             
             compositionView = new CompositionView(compositionPanel)
             {
                 ViewModel = new CompositionViewModel()
             };

             // locationsView = new LocationsView(locationsGlobe)
             // {
             //     ViewModel = new LocationsViewModel()
             // };
         }
    }
}