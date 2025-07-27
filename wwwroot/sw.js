// Service Worker para BaseApi PWA
const CACHE_NAME = 'baseapi-v1.0.0';
const OFFLINE_URL = '/offline.html';

// Archivos críticos para cachear
const CRITICAL_FILES = [
    '/',
    '/index.html',
    '/dashboard.html',
    '/css/main.css',
    '/js/auth.js',
    '/components/login-component.js',
    '/manifest.json',
    '/offline.html'
];

// Archivos de API que se pueden cachear
const API_CACHE_PATTERNS = [
    '/api/auth/login',
    '/api/auth/register',
    '/api/users'
];

// Instalación del Service Worker
self.addEventListener('install', (event) => {
    console.log('[SW] Installing Service Worker');
    
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => {
                console.log('[SW] Caching critical files');
                return cache.addAll(CRITICAL_FILES);
            })
            .then(() => {
                console.log('[SW] Critical files cached successfully');
                return self.skipWaiting();
            })
            .catch((error) => {
                console.error('[SW] Error caching critical files:', error);
            })
    );
});

// Activación del Service Worker
self.addEventListener('activate', (event) => {
    console.log('[SW] Activating Service Worker');
    
    event.waitUntil(
        caches.keys()
            .then((cacheNames) => {
                return Promise.all(
                    cacheNames.map((cacheName) => {
                        if (cacheName !== CACHE_NAME) {
                            console.log('[SW] Deleting old cache:', cacheName);
                            return caches.delete(cacheName);
                        }
                    })
                );
            })
            .then(() => {
                console.log('[SW] Service Worker activated');
                return self.clients.claim();
            })
    );
});

// Interceptar peticiones de red
self.addEventListener('fetch', (event) => {
    const { request } = event;
    const url = new URL(request.url);
    
    // Solo manejar peticiones del mismo origen
    if (url.origin !== location.origin) {
        return;
    }
    
    // Estrategia para páginas HTML
    if (request.destination === 'document') {
        event.respondWith(handleDocumentRequest(request));
        return;
    }
    
    // Estrategia para assets estáticos (CSS, JS, imágenes)
    if (request.destination === 'style' || 
        request.destination === 'script' || 
        request.destination === 'image') {
        event.respondWith(handleAssetRequest(request));
        return;
    }
    
    // Estrategia para API calls
    if (url.pathname.startsWith('/api/')) {
        event.respondWith(handleApiRequest(request));
        return;
    }
    
    // Para otras peticiones, intentar red primero
    event.respondWith(
        fetch(request)
            .catch(() => caches.match(request))
    );
});

// Manejar peticiones de documentos HTML
async function handleDocumentRequest(request) {
    try {
        // Intentar red primero
        const response = await fetch(request);
        
        // Si es exitosa, cachear y devolver
        if (response.status === 200) {
            const cache = await caches.open(CACHE_NAME);
            cache.put(request, response.clone());
            return response;
        }
        
        throw new Error('Network response not ok');
        
    } catch (error) {
        console.log('[SW] Network failed for document, trying cache');
        
        // Intentar cache
        const cachedResponse = await caches.match(request);
        if (cachedResponse) {
            return cachedResponse;
        }
        
        // Si no hay cache, mostrar página offline
        return caches.match(OFFLINE_URL);
    }
}

// Manejar peticiones de assets estáticos
async function handleAssetRequest(request) {
    try {
        // Cache first strategy para assets
        const cachedResponse = await caches.match(request);
        if (cachedResponse) {
            return cachedResponse;
        }
        
        // Si no está en cache, fetch y cachear
        const response = await fetch(request);
        if (response.status === 200) {
            const cache = await caches.open(CACHE_NAME);
            cache.put(request, response.clone());
        }
        
        return response;
        
    } catch (error) {
        console.log('[SW] Asset request failed:', request.url);
        // Para assets críticos, intentar una vez más desde cache
        return caches.match(request);
    }
}

// Manejar peticiones de API
async function handleApiRequest(request) {
    try {
        // Network first strategy para APIs
        const response = await fetch(request);
        
        // Cachear respuestas exitosas de GET
        if (response.status === 200 && request.method === 'GET') {
            const cache = await caches.open(CACHE_NAME);
            cache.put(request, response.clone());
        }
        
        return response;
        
    } catch (error) {
        console.log('[SW] API request failed, trying cache:', request.url);
        
        // Solo para GET requests, intentar cache
        if (request.method === 'GET') {
            const cachedResponse = await caches.match(request);
            if (cachedResponse) {
                return cachedResponse;
            }
        }
        
        // Para POST/PUT/DELETE, devolver error específico
        return new Response(
            JSON.stringify({
                error: 'Sin conexión a internet',
                message: 'Esta operación requiere conexión a internet',
                offline: true
            }),
            {
                status: 503,
                statusText: 'Service Unavailable',
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        );
    }
}

// Manejar mensajes del cliente
self.addEventListener('message', (event) => {
    const { type, payload } = event.data;
    
    switch (type) {
        case 'SKIP_WAITING':
            self.skipWaiting();
            break;
            
        case 'GET_VERSION':
            event.ports[0].postMessage({
                version: CACHE_NAME
            });
            break;
            
        case 'CLEAR_CACHE':
            caches.delete(CACHE_NAME)
                .then(() => {
                    event.ports[0].postMessage({
                        success: true,
                        message: 'Cache cleared successfully'
                    });
                })
                .catch((error) => {
                    event.ports[0].postMessage({
                        success: false,
                        error: error.message
                    });
                });
            break;
            
        default:
            console.log('[SW] Unknown message type:', type);
    }
});

// Manejar sincronización en background
self.addEventListener('sync', (event) => {
    console.log('[SW] Background sync triggered:', event.tag);
    
    if (event.tag === 'background-sync') {
        event.waitUntil(doBackgroundSync());
    }
});

// Función de sincronización en background
async function doBackgroundSync() {
    try {
        // Aquí se pueden enviar datos pendientes cuando vuelva la conexión
        console.log('[SW] Performing background sync');
        
        // Ejemplo: enviar datos pendientes de localStorage
        const clients = await self.clients.matchAll();
        clients.forEach(client => {
            client.postMessage({
                type: 'BACKGROUND_SYNC',
                message: 'Conexión restaurada, sincronizando datos...'
            });
        });
        
    } catch (error) {
        console.error('[SW] Background sync failed:', error);
    }
}

// Manejar notificaciones push (opcional)
self.addEventListener('push', (event) => {
    if (!event.data) {
        return;
    }
    
    const data = event.data.json();
    const options = {
        body: data.body || 'Nueva notificación de BaseApi',
        icon: '/images/icon-192x192.png',
        badge: '/images/icon-72x72.png',
        vibrate: [200, 100, 200],
        data: data.data || {},
        actions: [
            {
                action: 'open',
                title: 'Abrir',
                icon: '/images/action-open.png'
            },
            {
                action: 'close',
                title: 'Cerrar',
                icon: '/images/action-close.png'
            }
        ]
    };
    
    event.waitUntil(
        self.registration.showNotification(
            data.title || 'BaseApi',
            options
        )
    );
});

// Manejar clicks en notificaciones
self.addEventListener('notificationclick', (event) => {
    event.notification.close();
    
    if (event.action === 'open') {
        event.waitUntil(
            clients.openWindow('/dashboard.html')
        );
    }
});