const axios = require('axios');
const fs = require('fs');
const api = require('./apiService.js');
const Encryptor = require('./encryptor.js');

class jarvisAPI {
  constructor(apiKey, apiSecret) {
    this.apiKey = apiKey;
    this.encrypt = new Encryptor(apiSecret);
    this.jarvisUrl = 'https://www.jarvishomeautomation.com/';
  }

  /**
   * @function
   * @param {string} prompt - what to say in the audio file
   * @param {string} filename - name of audio file to save to 
   */
  getTTS(prompt, filename) {
    axios({
      method: 'post',
      url: `${this.jarvisUrl}/jarvis/v1/TTS`,
      data: {
        apiKey: this.apiKey,
        payload: this.encrypt.encrypt({ prompt, primer: null }),
        deviceID: null,
      },
      responseType: 'stream',
    }).then((response) => {
      // Pipe the stream to a file
      response.data.pipe(fs.createWriteStream(`${filename}.mp3`));
    });
  }

  /**
   * @function
   * @param {object} chatObj - has what a primer and prompt for the LLM to respond too
   * @param {string} chatObj.primer - Setup what the LLM is going to act like
   * @param {string} chatObj.prompt - What to send to the LLM
   * @returns {string} json string of prompt
   */
  chat(chatObj) {
    const payload = this.encrypt.encrypt(chatObj);
    const request = {
      apiKey: this.apiKey,
      payload,
      deviceID: null,
    };
    return new Promise((resolve, reject) => {
      api.callEndpoint(`${this.jarvisUrl}/jarvis/v1/Chat`, 'POST', request, (err, data) => {
        if (err) {
          reject(err);
        } else {
          resolve(data);
        }
      });
    });
  }

  /**
   * @function
   * @param {object} languageObj - has what type of language to respond in and the prompt
   * @param {string} languageObj.language - what language to respond in
   * @param {string} languageObj.prompt - what to respond to for generating code
   * @returns {string} json string of code
   */
  getCode(languageObj) {
    const payload = this.encrypt.encrypt(languageObj);
    const request = {
      apiKey: this.apiKey,
      payload,
      deviceID: null,
    };

    return new Promise((resolve, reject) => {
      api.callEndpoint(`${this.jarvisUrl}/jarvis/v1/GetCode`, 'POST', request, (err, data) => {
        if (err) {
          reject(err);
        } else {
          resolve(data);
        }
      });
    });
  }

  getSentiment() {}
}

module.exports = jarvisAPI;
