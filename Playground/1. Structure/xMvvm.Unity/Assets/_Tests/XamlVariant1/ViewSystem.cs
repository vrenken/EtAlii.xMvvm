namespace EtAlii.xMvvm
{
    using UnityEngine;

    public class ViewSystem : MonoBehaviour
    {
        [SerializeField]
        public GameObject loginPanel;

        public LoginView loginView;

        private void Start()
        {
            // This is just a simple method to wire up the View with ViewModels.
            // There are most probably better ways to do so.  

            loginView = new LoginView(loginPanel)
            {
                ViewModel = new LoginViewModel
                {
                    UserName = "john.doe@nomail.com",
                    Password = "123"
                }
            };
        }
    }
}