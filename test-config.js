const config = require('./commitlint.config.cjs');
console.log('Config loaded successfully!');
console.log('Number of type-enum values:', config.rules['type-enum'][2].length);
console.log('First 3 types:', config.rules['type-enum'][2].slice(0, 3).join(', '));
console.log('Extends:', config.extends);
