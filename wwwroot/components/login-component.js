class LoginComponent extends HTMLElement {
    constructor() {
        super();
        
        // Crear Shadow Root
        this.attachShadow({ mode: 'open' });
        
        // Estado del componente
        this.state = {
            isLoading: false,
            showRegister: false,
            error: null,
            success: null
        };
        
        this.render();
        this.attachEventListeners();
    }
    
    render() {
        const template = `
            <style>
                :host {
                    display: block;
                    width: 100%;
                    max-width: 400px;
                    margin: 0 auto;
                }
                
                .auth-card {
                    background: white;
                    border-radius: 12px;
                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    padding: 2rem;
                    margin: 2rem 0;
                }
                
                .auth-header {
                    text-align: center;
                    margin-bottom: 2rem;
                }
                
                .auth-header h2 {
                    color: #2c3e50;
                    margin: 0 0 0.5rem 0;
                    font-size: 1.8rem;
                }
                
                .auth-header p {
                    color: #7f8c8d;
                    margin: 0;
                }
                
                .form-group {
                    margin-bottom: 1.5rem;
                }
                
                .form-group label {
                    display: block;
                    margin-bottom: 0.5rem;
                    color: #34495e;
                    font-weight: 500;
                }
                
                .form-group input {
                    width: 100%;
                    padding: 0.75rem;
                    border: 2px solid #ecf0f1;
                    border-radius: 6px;
                    font-size: 1rem;
                    transition: border-color 0.3s ease;
                    box-sizing: border-box;
                }
                
                .form-group input:focus {
                    outline: none;
                    border-color: #3498db;
                }
                
                .form-group input.error {
                    border-color: #e74c3c;
                }
                
                .btn {
                    width: 100%;
                    padding: 0.75rem;
                    border: none;
                    border-radius: 6px;
                    font-size: 1rem;
                    font-weight: 500;
                    cursor: pointer;
                    transition: background-color 0.3s ease;
                }
                
                .btn-primary {
                    background-color: #3498db;
                    color: white;
                }
                
                .btn-primary:hover:not(:disabled) {
                    background-color: #2980b9;
                }
                
                .btn-secondary {
                    background-color: #95a5a6;
                    color: white;
                    margin-top: 0.5rem;
                }
                
                .btn-secondary:hover:not(:disabled) {
                    background-color: #7f8c8d;
                }
                
                .btn:disabled {
                    opacity: 0.6;
                    cursor: not-allowed;
                }
                
                .alert {
                    padding: 0.75rem;
                    border-radius: 6px;
                    margin-bottom: 1rem;
                    font-size: 0.9rem;
                }
                
                .alert-error {
                    background-color: #ffeaea;
                    border: 1px solid #e74c3c;
                    color: #c0392b;
                }
                
                .alert-success {
                    background-color: #eafaf1;
                    border: 1px solid #27ae60;
                    color: #1e8449;
                }
                
                .toggle-form {
                    text-align: center;
                    margin-top: 1.5rem;
                    padding-top: 1.5rem;
                    border-top: 1px solid #ecf0f1;
                }
                
                .toggle-form a {
                    color: #3498db;
                    text-decoration: none;
                    font-weight: 500;
                }
                
                .toggle-form a:hover {
                    text-decoration: underline;
                }
                
                .loading {
                    display: inline-block;
                    width: 16px;
                    height: 16px;
                    border: 2px solid #ffffff;
                    border-radius: 50%;
                    border-top-color: transparent;
                    animation: spin 1s ease-in-out infinite;
                    margin-right: 0.5rem;
                }
                
                @keyframes spin {
                    to { transform: rotate(360deg); }
                }
                
                .hidden {
                    display: none;
                }
            </style>
            
            <div class="auth-card">
                ${this.state.error ? `<div class="alert alert-error">${this.state.error}</div>` : ''}
                ${this.state.success ? `<div class="alert alert-success">${this.state.success}</div>` : ''}
                
                <!-- Formulario de Login -->
                <div id="login-form" class="${this.state.showRegister ? 'hidden' : ''}">
                    <div class="auth-header">
                        <h2>Iniciar Sesión</h2>
                        <p>Ingresa tus credenciales para acceder al sistema</p>
                    </div>
                    
                    <form id="loginForm">
                        <div class="form-group">
                            <label for="loginUsername">Usuario</label>
                            <input type="text" id="loginUsername" name="username" required>
                        </div>
                        
                        <div class="form-group">
                            <label for="loginPassword">Contraseña</label>
                            <input type="password" id="loginPassword" name="password" required>
                        </div>
                        
                        <button type="submit" class="btn btn-primary" ${this.state.isLoading ? 'disabled' : ''}>
                            ${this.state.isLoading ? '<span class="loading"></span>' : ''}
                            ${this.state.isLoading ? 'Iniciando Sesión...' : 'Iniciar Sesión'}
                        </button>
                    </form>
                    
                    <div class="toggle-form">
                        <p>¿No tienes cuenta? <a href="#" id="showRegister">Regístrate aquí</a></p>
                    </div>
                </div>
                
                <!-- Formulario de Registro -->
                <div id="register-form" class="${!this.state.showRegister ? 'hidden' : ''}">
                    <div class="auth-header">
                        <h2>Registrarse</h2>
                        <p>Crea una nueva cuenta en el sistema</p>
                    </div>
                    
                    <form id="registerForm">
                        <div class="form-group">
                            <label for="registerUsername">Usuario</label>
                            <input type="text" id="registerUsername" name="username" required>
                        </div>
                        
                        <div class="form-group">
                            <label for="registerEmail">Email</label>
                            <input type="email" id="registerEmail" name="email" required>
                        </div>
                        
                        <div class="form-group">
                            <label for="registerFirstName">Nombre</label>
                            <input type="text" id="registerFirstName" name="firstName">
                        </div>
                        
                        <div class="form-group">
                            <label for="registerLastName">Apellido</label>
                            <input type="text" id="registerLastName" name="lastName">
                        </div>
                        
                        <div class="form-group">
                            <label for="registerPassword">Contraseña</label>
                            <input type="password" id="registerPassword" name="password" required minlength="6">
                        </div>
                        
                        <div class="form-group">
                            <label for="confirmPassword">Confirmar Contraseña</label>
                            <input type="password" id="confirmPassword" name="confirmPassword" required minlength="6">
                        </div>
                        
                        <button type="submit" class="btn btn-primary" ${this.state.isLoading ? 'disabled' : ''}>
                            ${this.state.isLoading ? '<span class="loading"></span>' : ''}
                            ${this.state.isLoading ? 'Registrando...' : 'Registrarse'}
                        </button>
                        
                        <button type="button" class="btn btn-secondary" id="showLogin">
                            Volver al Login
                        </button>
                    </form>
                </div>
            </div>
        `;
        
        this.shadowRoot.innerHTML = template;
    }
    
    attachEventListeners() {
        // Toggle entre login y registro
        const showRegisterBtn = this.shadowRoot.getElementById('showRegister');
        const showLoginBtn = this.shadowRoot.getElementById('showLogin');
        
        showRegisterBtn?.addEventListener('click', (e) => {
            e.preventDefault();
            this.toggleForm(true);
        });
        
        showLoginBtn?.addEventListener('click', (e) => {
            e.preventDefault();
            this.toggleForm(false);
        });
        
        // Formulario de login
        const loginForm = this.shadowRoot.getElementById('loginForm');
        loginForm?.addEventListener('submit', (e) => this.handleLogin(e));
        
        // Formulario de registro
        const registerForm = this.shadowRoot.getElementById('registerForm');
        registerForm?.addEventListener('submit', (e) => this.handleRegister(e));
    }
    
    toggleForm(showRegister) {
        this.state.showRegister = showRegister;
        this.state.error = null;
        this.state.success = null;
        this.render();
        this.attachEventListeners();
    }
    
    async handleLogin(e) {
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const loginData = {
            username: formData.get('username'),
            password: formData.get('password')
        };
        
        this.setLoading(true);
        this.clearMessages();
        
        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(loginData)
            });
            
            const result = await response.json();
            
            if (response.ok) {
                this.showSuccess('¡Login exitoso! Redirigiendo...');
                localStorage.setItem('authToken', result.token);
                localStorage.setItem('username', result.username);
                
                // Disparar evento personalizado para notificar login exitoso
                this.dispatchEvent(new CustomEvent('loginSuccess', {
                    detail: result,
                    bubbles: true
                }));
                
                // Redirigir después de 1 segundo
                setTimeout(() => {
                    window.location.href = '/dashboard.html';
                }, 1000);
                
            } else {
                this.showError(result.message || 'Error en el login');
            }
            
        } catch (error) {
            this.showError('Error de conexión. Intenta nuevamente.');
            console.error('Login error:', error);
        } finally {
            this.setLoading(false);
        }
    }
    
    async handleRegister(e) {
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const password = formData.get('password');
        const confirmPassword = formData.get('confirmPassword');
        
        if (password !== confirmPassword) {
            this.showError('Las contraseñas no coinciden');
            return;
        }
        
        const registerData = {
            username: formData.get('username'),
            email: formData.get('email'),
            firstName: formData.get('firstName'),
            lastName: formData.get('lastName'),
            password: password,
            confirmPassword: confirmPassword
        };
        
        this.setLoading(true);
        this.clearMessages();
        
        try {
            const response = await fetch('/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(registerData)
            });
            
            const result = await response.json();
            
            if (response.ok) {
                this.showSuccess('¡Registro exitoso! Redirigiendo...');
                localStorage.setItem('authToken', result.token);
                localStorage.setItem('username', result.username);
                
                // Disparar evento personalizado
                this.dispatchEvent(new CustomEvent('registerSuccess', {
                    detail: result,
                    bubbles: true
                }));
                
                // Redirigir después de 1 segundo
                setTimeout(() => {
                    window.location.href = '/dashboard.html';
                }, 1000);
                
            } else {
                this.showError(result.message || 'Error en el registro');
            }
            
        } catch (error) {
            this.showError('Error de conexión. Intenta nuevamente.');
            console.error('Register error:', error);
        } finally {
            this.setLoading(false);
        }
    }
    
    setLoading(loading) {
        this.state.isLoading = loading;
        this.render();
        this.attachEventListeners();
    }
    
    showError(message) {
        this.state.error = message;
        this.state.success = null;
        this.render();
        this.attachEventListeners();
    }
    
    showSuccess(message) {
        this.state.success = message;
        this.state.error = null;
        this.render();
        this.attachEventListeners();
    }
    
    clearMessages() {
        this.state.error = null;
        this.state.success = null;
        this.render();
        this.attachEventListeners();
    }
}

// Registrar el componente personalizado
customElements.define('login-component', LoginComponent);