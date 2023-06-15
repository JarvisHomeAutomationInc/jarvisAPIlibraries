const https = require('https');

exports.callEndpoint = function (endpoint, method, body, callback) {
  const options = {
    method, // This can be 'GET', 'POST', 'PUT', 'DELETE'
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const req = https
    .request(endpoint, options, (res) => {
      let data = '';

      // Concatenate the chunks of data.
      res.on('data', (chunk) => {
        data += chunk;
      });

      // When all data has been received, parse it as JSON.
      res.on('end', () => {
        try {
          const parsedData = JSON.parse(data);
          callback(null, parsedData);
        } catch (e) {
          callback(e);
        }
      });
    })
    .on('error', (err) => {
      callback(err);
    });

  // If it's a POST or PUT request, you might want to send some data.
  if (method === 'POST' || method === 'PUT') {
    const postData = JSON.stringify(body);
    req.write(postData);
  }

  req.end();
};
