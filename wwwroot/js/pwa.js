// PWA Installation and Management
class PWAManager {
    constructor() {
        this.deferredPrompt = null;
        this.isInstalled = false;
        this.serviceWorker = null;
        
        this.init();
    }
    
    async init() {
        // Verificar si la app ya está instalada
        this.checkInstallationStatus();
        
        // Registrar Service Worker
        await this.registerServiceWorker();
        
        // Configurar eventos de instalación
        this.setupInstallationEvents();
        
        // Configurar eventos de actualización
        this.setupUpdateEvents();
        
        // Mostrar banner de instalación si es apropiado
        this.setupInstallationBanner();
    }
    
    checkInstallationStatus() {
        // Verificar si está ejecutándose como PWA
        this.isInstalled = window.matchMedia('(display-mode: standalone)').matches ||
                          window.navigator.standalone ||
                          document.referrer.includes('android-app://');
        
        if (this.isInstalled) {
            console.log('[PWA] App is running as installed PWA');
            document.body.classList.add('pwa-installed');
        }
    }
    
    async registerServiceWorker() {
        if ('serviceWorker' in navigator) {
            try {
                const registration = await navigator.serviceWorker.register('/sw.js', {
                    scope: '/'
                });
                
                this.serviceWorker = registration;
                console.log('[PWA] Service Worker registered successfully');
                
                // Verificar actualizaciones
                registration.addEventListener('updatefound', () => {
                    this.handleServiceWorkerUpdate(registration);
                });
                
                return registration;
                
            } catch (error) {
                console.error('[PWA] Service Worker registration failed:', error);
            }
        }
    }
    
    setupInstallationEvents() {
        // Escuchar evento beforeinstallprompt
        window.addEventListener('beforeinstallprompt', (e) => {
            console.log('[PWA] Install prompt available');
            
            // Prevenir que se muestre automáticamente
            e.preventDefault();
            
            // Guardar el evento para usarlo después
            this.deferredPrompt = e;
            
            // Mostrar botón de instalación personalizado
            this.showInstallButton();
        });
        
        // Escuchar cuando la app es instalada
        window.addEventListener('appinstalled', (e) => {
            console.log('[PWA] App was installed');
            this.isInstalled = true;
            this.hideInstallButton();
            this.showInstalledMessage();
        });
    }
    
    setupUpdateEvents() {
        // Escuchar mensajes del Service Worker
        navigator.serviceWorker.addEventListener('message', (event) => {
            const { type, message } = event.data;
            
            switch (type) {
                case 'BACKGROUND_SYNC':
                    this.showNotification('info', message);
                    break;
                    
                case 'UPDATE_AVAILABLE':
                    this.showUpdateAvailable();
                    break;
                    
                default:
                    console.log('[PWA] SW Message:', event.data);
            }
        });
    }
    
    setupInstallationBanner() {
        // Crear banner de instalación
        const banner = document.createElement('div');
        banner.id = 'pwa-install-banner';
        banner.className = 'pwa-banner hidden';
        banner.innerHTML = `
            <div class="pwa-banner-content">
                <div class="pwa-banner-text">
                    <strong>¡Instala BaseApi!</strong>
                    <span>Accede más rápido y úsala sin conexión</span>
                </div>
                <div class="pwa-banner-actions">
                    <button id="pwa-install-btn" class="btn btn-primary">Instalar</button>
                    <button id="pwa-dismiss-btn" class="btn btn-secondary">No ahora</button>
                </div>
            </div>
        `;
        
        document.body.appendChild(banner);
        
        // Configurar eventos de los botones
        document.getElementById('pwa-install-btn')?.addEventListener('click', () => {
            this.installPWA();
        });
        
        document.getElementById('pwa-dismiss-btn')?.addEventListener('click', () => {
            this.hideInstallButton();
        });
    }
    
    showInstallButton() {
        const banner = document.getElementById('pwa-install-banner');
        if (banner && !this.isInstalled) {
            banner.classList.remove('hidden');
            
            // Auto-hide después de 30 segundos
            setTimeout(() => {
                this.hideInstallButton();
            }, 30000);
        }
    }
    
    hideInstallButton() {
        const banner = document.getElementById('pwa-install-banner');
        if (banner) {
            banner.classList.add('hidden');
        }
    }
    
    async installPWA() {
        if (!this.deferredPrompt) {
            console.log('[PWA] No install prompt available');
            return;
        }
        
        try {
            // Mostrar el prompt de instalación
            this.deferredPrompt.prompt();
            
            // Esperar la respuesta del usuario
            const { outcome } = await this.deferredPrompt.userChoice;
            
            console.log('[PWA] Install prompt result:', outcome);
            
            if (outcome === 'accepted') {
                this.showNotification('success', '¡Aplicación instalada correctamente!');
            } else {
                this.showNotification('info', 'Instalación cancelada');
            }
            
            // Limpiar el prompt
            this.deferredPrompt = null;
            this.hideInstallButton();
            
        } catch (error) {
            console.error('[PWA] Installation failed:', error);
            this.showNotification('error', 'Error durante la instalación');
        }
    }
    
    handleServiceWorkerUpdate(registration) {
        const newWorker = registration.installing;
        
        newWorker.addEventListener('statechange', () => {
            if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                console.log('[PWA] New service worker installed, update available');
                this.showUpdateAvailable();
            }
        });
    }
    
    showUpdateAvailable() {
        const updateBanner = document.createElement('div');
        updateBanner.id = 'pwa-update-banner';
        updateBanner.className = 'pwa-update-banner';
        updateBanner.innerHTML = `
            <div class="pwa-update-content">
                <span>Nueva versión disponible</span>
                <button id="pwa-update-btn" class="btn btn-primary">Actualizar</button>
                <button id="pwa-update-dismiss" class="btn btn-secondary">Después</button>
            </div>
        `;
        
        document.body.appendChild(updateBanner);
        
        document.getElementById('pwa-update-btn')?.addEventListener('click', () => {
            this.applyUpdate();
        });
        
        document.getElementById('pwa-update-dismiss')?.addEventListener('click', () => {
            updateBanner.remove();
        });
    }
    
    async applyUpdate() {
        if (this.serviceWorker) {
            // Enviar mensaje para activar el nuevo service worker
            this.serviceWorker.waiting?.postMessage({ type: 'SKIP_WAITING' });
            
            // Recargar la página
            window.location.reload();
        }
    }
    
    showInstalledMessage() {
        this.showNotification('success', '¡BaseApi instalada correctamente! Ya puedes usarla desde tu pantalla de inicio.');
    }
    
    showNotification(type, message) {
        // Crear notificación toast
        const notification = document.createElement('div');
        notification.className = `pwa-notification pwa-notification-${type}`;
        notification.textContent = message;
        
        document.body.appendChild(notification);
        
        // Mostrar con animación
        setTimeout(() => {
            notification.classList.add('show');
        }, 100);
        
        // Auto-remove después de 5 segundos
        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => {
                notification.remove();
            }, 300);
        }, 5000);
    }
    
    // Métodos públicos para usar desde otros scripts
    async checkForUpdates() {
        if (this.serviceWorker) {
            await this.serviceWorker.update();
        }
    }
    
    async clearCache() {
        if (this.serviceWorker) {
            const messageChannel = new MessageChannel();
            
            return new Promise((resolve) => {
                messageChannel.port1.onmessage = (event) => {
                    resolve(event.data);
                };
                
                this.serviceWorker.active?.postMessage(
                    { type: 'CLEAR_CACHE' },
                    [messageChannel.port2]
                );
            });
        }
    }
    
    getInstallationStatus() {
        return {
            isInstalled: this.isInstalled,
            canInstall: !!this.deferredPrompt,
            hasServiceWorker: !!this.serviceWorker
        };
    }
}

// Estilos CSS para PWA
const pwaStyles = `
    .pwa-banner {
        position: fixed;
        bottom: 0;
        left: 0;
        right: 0;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        padding: 1rem;
        transform: translateY(100%);
        transition: transform 0.3s ease;
        z-index: 1000;
        box-shadow: 0 -4px 6px rgba(0, 0, 0, 0.1);
    }
    
    .pwa-banner:not(.hidden) {
        transform: translateY(0);
    }
    
    .pwa-banner-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
        max-width: 800px;
        margin: 0 auto;
    }
    
    .pwa-banner-text strong {
        display: block;
        font-size: 1rem;
        margin-bottom: 0.25rem;
    }
    
    .pwa-banner-text span {
        font-size: 0.9rem;
        opacity: 0.9;
    }
    
    .pwa-banner-actions {
        display: flex;
        gap: 0.5rem;
    }
    
    .pwa-banner .btn {
        padding: 0.5rem 1rem;
        border: 1px solid rgba(255, 255, 255, 0.3);
        border-radius: 4px;
        font-size: 0.9rem;
        cursor: pointer;
        transition: all 0.3s ease;
    }
    
    .pwa-banner .btn-primary {
        background: white;
        color: #667eea;
    }
    
    .pwa-banner .btn-secondary {
        background: transparent;
        color: white;
    }
    
    .pwa-update-banner {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        background: #f39c12;
        color: white;
        padding: 0.75rem;
        z-index: 1001;
        animation: slideDown 0.3s ease;
    }
    
    .pwa-update-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
        max-width: 800px;
        margin: 0 auto;
    }
    
    .pwa-notification {
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        border-radius: 6px;
        color: white;
        font-weight: 500;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        z-index: 1002;
        max-width: 300px;
    }
    
    .pwa-notification.show {
        transform: translateX(0);
    }
    
    .pwa-notification-success {
        background: #27ae60;
    }
    
    .pwa-notification-error {
        background: #e74c3c;
    }
    
    .pwa-notification-info {
        background: #3498db;
    }
    
    .pwa-installed {
        /* Estilos para cuando la app está instalada */
    }
    
    @keyframes slideDown {
        from { transform: translateY(-100%); }
        to { transform: translateY(0); }
    }
    
    @media (max-width: 768px) {
        .pwa-banner-content {
            flex-direction: column;
            gap: 1rem;
            text-align: center;
        }
        
        .pwa-update-content {
            flex-direction: column;
            gap: 0.5rem;
            text-align: center;
        }
        
        .pwa-notification {
            right: 10px;
            left: 10px;
            max-width: none;
        }
    }
`;

// Inyectar estilos
const styleSheet = document.createElement('style');
styleSheet.textContent = pwaStyles;
document.head.appendChild(styleSheet);

// Inicializar PWA Manager cuando el DOM esté listo
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.pwaManager = new PWAManager();
    });
} else {
    window.pwaManager = new PWAManager();
}

// Exportar para uso global
window.PWAManager = PWAManager;