const express = require("express");
const path = require("path");
require('dotenv').config();

const basePath = '';
const app = express();

app.use(basePath + "/", express.static(path.resolve(__dirname + "/build")));

app.get("*", (request, response) => {
    response.sendFile(path.resolve(__dirname + "/build/index.html"));
});

app.listen(process.env.PORT);