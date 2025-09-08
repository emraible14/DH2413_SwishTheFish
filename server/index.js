const crypto = require("crypto");
const express = require("express");
const { createServer } = require("http");
const WebSocket = require("ws");

const app = express();
const port = 3001;

const server = createServer(app);
const wss = new WebSocket.Server({ server });

let unityClient = null;

let numFish = 0;

const { networkInterfaces } = require("os");

const nets = networkInterfaces();
const results = Object.create(null); // Or just '{}', an empty object

for (const name of Object.keys(nets)) {
  for (const net of nets[name]) {
    // Skip over non-IPv4 and internal (i.e. 127.0.0.1) addresses
    // 'IPv4' is in Node <= 17, from 18 it's a number 4 or 6
    const familyV4Value = typeof net.family === "string" ? "IPv4" : 4;
    if (net.family === familyV4Value && !net.internal) {
      if (!results[name]) {
        results[name] = [];
      }
      results[name].push(net.address);
    }
  }
}

function addFish(ws) {
  numFish++;
  ws.send("fishAdded");
  if (unityClient) {
    unityClient.send("fishAdded");
  }
}

function removeFish(ws) {
  numFish--;
  ws.send("fishRemoved");
}

wss.on("connection", function (ws) {
  console.log("client joined.");

  ws.on("message", function (data) {
    if (typeof data === "string") {
      // client sent a string
      console.log("string received from client -> '" + data + "'");
      if (data === "addFish") {
        addFish(ws);
      } else if (data === "removeFish") {
        removeFish(ws);
      } else if (data === "authenticate") {
        unityClient = ws;
      }
    } else {
      console.log(
        "binary received from client -> " + Array.from(data).join(", ") + ""
      );
    }
  });

  ws.on("close", function () {
    console.log("client left.");
    // clearInterval(textInterval);
    // clearInterval(binaryInterval);
  });
});

app.get("/", (req, res) => {
  res.sendFile(__dirname + "/index.html");
});

server.listen(port, function () {
  console.log(`Listening on http://localhost:${port}`);
  // print only url with green color
  console.log(
    "\x1b[32m%s\x1b[0m",
    `Listening on local network http://${results["en0"][0]}:${port}`
  );
});
