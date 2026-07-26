## Purpose

Create consistent REST endpoints.

---

## Rules

GET
- Retrieve data.

POST
- Create resources.

PUT
- Update resources.

DELETE
- Remove resources.

---

## Responses

200 OK
201 Created
204 No Content
400 Bad Request
401 Unauthorized
404 Not Found
500 Internal Server Error

---

## Controllers
- Keep controllers thin.
- Delegate work to the Application layer.