
# Study Buddy's Backend - API Documentation

Welcome to the Study Buddy's Backend project! This repository contains the backend API for the Study Buddy's platform.

## API Endpoints

Here are some of the key API endpoints available in the backend:

### Get All Users
**Endpoint**: `GET https://study-buddys-backend.azurewebsites.net/User/getAllUsers`

This endpoint retrieves a list of all users from the backend.

# API down time

- dates [4/7/25]/[4/8/25]

#### Example Request:
```http
GET https://study-buddys-backend.azurewebsites.net/User/getAllUsers
```

#### Example Response:
```json
{
  "success": true,
  "users": [
    {
      "id": 1,
      "username": "string",
      "ownedCommunitys": [],
      "joinedCommunitys": [],
      "communityRequests": []
    }
  ]
}
```

---

### Get All Communities
**Endpoint**: `GET https://study-buddys-backend.azurewebsites.net/Community/getAllCommunities`

This endpoint retrieves a list of all communities from the backend.

#### Example Request:
```http
GET https://study-buddys-backend.azurewebsites.net/Community/getAllCommunities
```

#### Example Response:
```json
[
  "success": true,
  "communities": [
    {
      "id": 1,
      "communityOwnerID": 0,
      "isCommunityOwner": false,
      "communityIsPublic": false,
      "communityIsDeleted": true,
      "communityOwnerName": "king von",
      "communityName": "Welcome to the World",
      "communitySubject": "safety",
      "communityMemberCount": 0,
      "communityMembers": [
        {
          "id": 1,
          "userId": 0,
          "role": "string"
        },
        {
          "id": 2,
          "userId": 1,
          "role": "student"
        }
      ],
      "communityRequests": [0],
      "communityDifficulty": "hard",
      "communityDescription": "helping future"
    }
]
```

## Repository Link
The repository for this project has been moved to the new GitHub link:

[https://github.com/rob2fresh4this/study-buddys-backend-v2](https://github.com/rob2fresh4this/study-buddys-backend-v2)

Please make sure to reference this new link for future development, issues, and contributions.


Older Github Link:

[https://github.com/rob2fresh4this/Study-Buddys-Backend](https://github.com/rob2fresh4this/Study-Buddys-Backend)

---

Happy coding! 
