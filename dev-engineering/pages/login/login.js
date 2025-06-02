import { isLoggedIn, setLoggedIn } from '../../index.js';
import { updateDisabledButtons } from '../../index.js';
import { officerLogin, setOfficerLogin } from '../../index.js';
import { currentView, setCurrentView } from '../../index.js';

export function init() {

    setCurrentView('login');

    const loginButton = document.getElementById('loginButton');
    const logoutButton = document.getElementById('logoutButton');
    logoutButton.disabled = !isLoggedIn;
    const officerLoginInput = document.getElementById('officerLogin');

    loginButton.onclick = (event) => {
        event.preventDefault();
        setLoggedIn(true);
        updateDisabledButtons();
        
        setOfficerLogin(officerLoginInput.value);
        loginButton.disabled = true;
        logoutButton.disabled = false;

        const welcomeMessage = document.getElementById('welcomeMessage');
        welcomeMessage.textContent = `Welcome, ${officerLoginInput.value}`;
    };

    if (isLoggedIn) {
        const welcomeMessage = document.getElementById('welcomeMessage');
        welcomeMessage.textContent = `Welcome, ${officerLogin}`;
    }

    logoutButton.onclick = (event) => {
        event.preventDefault();
        setLoggedIn(false);
        updateDisabledButtons();
        setOfficerLogin("");

        loginButton.disabled = false;
        logoutButton.disabled = true;
        
        const welcomeMessage = document.getElementById('welcomeMessage');
        welcomeMessage.textContent = "You have logged out.";
    }
}