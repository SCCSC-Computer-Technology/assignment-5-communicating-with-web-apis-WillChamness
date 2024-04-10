[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/KoyWxUdl)

# MVC Web App
The web app contains a simple form to CRUD. The web app will GET, POST, PUT, and DELETE to the API appropriately. 
Upon doing so, the page will change to a view where the user can see the results of the request or an error message
if appropriate. Because the API does not contain an endpoint to PATCH, there is no functionally to send a PATCH request.

Run the web app and go to `https://localhost:5001` in a web browser of your choice.

# Student Profile API
The student profile API is almost enitrely unchanged from the in-class example. There are some small modifications to 
`Program.cs`, but the controller is completely untouched. The API is to be ran as a separate process from the web app.

The API listens on `localhost:5002` via HTTPS for requests. There is only one route: `/api/StudentProfiles/`. Furthermore,
the API accesses an in-memory database, so restarting the API will delete all data.
