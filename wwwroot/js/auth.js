// Utilidades de autenticación
class AuthManager {
    constructor() {
        // Intentar con ambos nombres para compatibilidad
        this.token = localStorage.getItem('token') || localStorage.getItem('authToken');
        this.username = localStorage.getItem('username');
    }
    
    // Verificar si el usuario está autenticado
    isAuthenticated() {
        return !!this.token;
    }
    
    // Obtener el token
    getToken() {
        return this.token;
    }
    
    // Obtener el nombre de usuario
    getUsername() {
        return this.username;
    }
    
    // Guardar datos de autenticación
    setAuthData(token, username) {
        this.token = token;
        this.username = username;
        // Guardar con ambos nombres para compatibilidad
        localStorage.setItem('token', token);
        localStorage.setItem('authToken', token);
        localStorage.setItem('username', username);
    }
    
    // Limpiar datos de autenticación
    clearAuthData() {
        this.token = null;
        this.username = null;
        // Limpiar ambos nombres
        localStorage.removeItem('token');
        localStorage.removeItem('authToken');
        localStorage.removeItem('username');
    }
    
    // Realizar petición autenticada
    async authenticatedFetch(url, options = {}) {
        if (!this.token) {
            throw new Error('No hay token de autenticación');
        }
        
        const defaultHeaders = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${this.token}`
        };
        
        const mergedOptions = {
            ...options,
            headers: {
                ...defaultHeaders,
                ...options.headers
            }
        };
        
        const response = await fetch(url, mergedOptions);
        
        // Si el token expiró, redirigir al login
        if (response.status === 401) {
            this.clearAuthData();
            window.location.href = '/';
            return;
        }
        
        return response;
    }
    
    // Cerrar sesión
    logout() {
        this.clearAuthData();
        window.location.href = '/';
    }
}

// Crear instancia global
window.authManager = new AuthManager();

// Funciones globales de utilidad
window.isAuthenticated = () => window.authManager.isAuthenticated();
window.getAuthToken = () => window.authManager.getToken();
window.getUsername = () => window.authManager.getUsername();
window.logout = () => window.authManager.logout();

// Verificar autenticación al cargar la página
document.addEventListener('DOMContentLoaded', () => {
    // Si estamos en una página que requiere autenticación y no estamos autenticados
    const protectedPages = ['/dashboard.html', '/users.html'];
    const currentPage = window.location.pathname;
    
    if (protectedPages.includes(currentPage) && !window.authManager.isAuthenticated()) {
        window.location.href = '/';
        return;
    }
    
    // Si estamos en la página de login y ya estamos autenticados
    if (currentPage === '/' && window.authManager.isAuthenticated()) {
        window.location.href = '/dashboard.html';
        return;
    }
});

// Escuchar eventos de login exitoso
document.addEventListener('loginSuccess', (event) => {
    console.log('Login exitoso:', event.detail);
});

document.addEventListener('registerSuccess', (event) => {
    console.log('Registro exitoso:', event.detail);
});