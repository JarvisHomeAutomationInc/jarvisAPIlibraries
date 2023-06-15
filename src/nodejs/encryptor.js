const crypto = require('crypto');

class JsonEncryptor {
  constructor(secretKey) {
    this.secretKey = crypto.createHash('sha256').update(String(secretKey)).digest('base64').substr(0, 32);
    this.algorithm = 'aes-256-cbc';
  }

  encrypt(json) {
    const iv = crypto.randomBytes(16);
    const cipher = crypto.createCipheriv(this.algorithm, this.secretKey, iv);
    const jsonString = JSON.stringify(json);
    const encrypted = Buffer.concat([cipher.update(jsonString, 'utf8'), cipher.final()]);
    return `${iv.toString('hex')}:${encrypted.toString('hex')}`;
  }

  decrypt(encrypted) {
    const textParts = encrypted.split(':');
    const iv = Buffer.from(textParts.shift(), 'hex');
    const encryptedText = Buffer.from(textParts.join(':'), 'hex');
    const decipher = crypto.createDecipheriv(this.algorithm, this.secretKey, iv);
    const decrypted = Buffer.concat([decipher.update(encryptedText), decipher.final()]);
    return JSON.parse(decrypted.toString());
  }
}

module.exports = JsonEncryptor;
