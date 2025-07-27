// Script para generar iconos PWA usando Node.js
const fs = require('fs');
const { createCanvas } = require('canvas');

const iconSizes = [16, 32, 72, 96, 128, 144, 152, 192, 384, 512];

function createIcon(size) {
    const canvas = createCanvas(size, size);
    const ctx = canvas.getContext('2d');
    
    // Crear gradiente
    const gradient = ctx.createLinearGradient(0, 0, size, size);
    gradient.addColorStop(0, '#667eea');
    gradient.addColorStop(1, '#764ba2');
    
    // Fondo con gradiente y bordes redondeados
    const radius = size * 0.125;
    ctx.fillStyle = gradient;
    roundRect(ctx, 0, 0, size, size, radius);
    ctx.fill();
    
    // Texto "BA"
    ctx.fillStyle = 'white';
    ctx.font = `bold ${size * 0.375}px Arial`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText('BA', size / 2, size * 0.68);
    
    // Decoración superior
    ctx.fillStyle = 'rgba(255, 255, 255, 0.3)';
    ctx.beginPath();
    ctx.arc(size / 2, size * 0.31, size * 0.104, 0, 2 * Math.PI);
    ctx.fill();
    
    ctx.fillStyle = 'rgba(255, 255, 255, 0.2)';
    const rectSize = size * 0.208;
    const rectRadius = size * 0.042;
    roundRect(ctx, (size - rectSize) / 2, size * 0.208, rectSize, rectSize, rectRadius);
    ctx.fill();
    
    return canvas;
}

function roundRect(ctx, x, y, width, height, radius) {
    ctx.beginPath();
    ctx.moveTo(x + radius, y);
    ctx.lineTo(x + width - radius, y);
    ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
    ctx.lineTo(x + width, y + height - radius);
    ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
    ctx.lineTo(x + radius, y + height);
    ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
    ctx.lineTo(x, y + radius);
    ctx.quadraticCurveTo(x, y, x + radius, y);
    ctx.closePath();
}

// Crear carpeta si no existe
if (!fs.existsSync('./wwwroot/images')) {
    fs.mkdirSync('./wwwroot/images', { recursive: true });
}

// Generar todos los iconos
iconSizes.forEach(size => {
    const canvas = createIcon(size);
    const buffer = canvas.toBuffer('image/png');
    
    fs.writeFileSync(`./wwwroot/images/icon-${size}x${size}.png`, buffer);
    console.log(`✅ Creado: icon-${size}x${size}.png`);
});

console.log('🎉 Todos los iconos PWA han sido generados exitosamente!');