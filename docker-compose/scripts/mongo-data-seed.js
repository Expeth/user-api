// password is 123456aF1!
var users = [
    { _id: "00000000-0000-0000-0000-000000000001", Username: "testuser1", FirstName: "test1", LastName: "test1", MiddleName: "test1", Email: "testuser1@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" },
    { _id: "00000000-0000-0000-0000-000000000002", Username: "testuser2", FirstName: "test2", LastName: "test2", MiddleName: "test2", Email: "testuser2@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" },
    { _id: "00000000-0000-0000-0000-000000000003", Username: "testuser3", FirstName: "test3", LastName: "test3", MiddleName: "test3", Email: "testuser3@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" },
    { _id: "00000000-0000-0000-0000-000000000004", Username: "testuser4", FirstName: "test4", LastName: "test4", MiddleName: "test4", Email: "testuser4@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" }
];

db.users.remove({ });
db.users.insert(users);

var sessions = [
    { _id: "39459de2-336d-4493-980e-97d6bf2f531b", UserId: "00000000-0000-0000-0000-000000000001", StartTime: "2021-12-16T12:35:37", EndTime: null },
    { _id: "ba2c0c85-f201-49d1-aae0-2f89a85bb895", UserId: "00000000-0000-0000-0000-000000000001", StartTime: "2021-12-16T12:35:37", EndTime: "2021-12-16T12:35:37" },
]

db.sessions.remove({ });
db.sessions.insert(sessions);

var refreshTokens = [
    { _id: "00000000-0000-0000-0000-000000000101", UserId: "00000000-0000-0000-0000-000000000001", SessionId: "39459de2-336d-4493-980e-97d6bf2f531b", IssuedAt: "2021-12-16T12:35:37", ExpiresAt: "2099-12-16T12:35:37", IsUsed: false, IsDeclined: false },
    { _id: "00000000-0000-0000-0000-000000000102", UserId: "00000000-0000-0000-0000-000000000001", SessionId: "39459de2-336d-4493-980e-97d6bf2f531b", IssuedAt: "2021-12-16T12:35:37", ExpiresAt: "2099-12-16T12:35:37", IsUsed: true, IsDeclined: false },
    { _id: "00000000-0000-0000-0000-000000000103", UserId: "00000000-0000-0000-0000-000000000001", SessionId: "39459de2-336d-4493-980e-97d6bf2f531b", IssuedAt: "2021-12-16T12:35:37", ExpiresAt: "2099-12-16T12:35:37", IsUsed: false, IsDeclined: true },
    { _id: "00000000-0000-0000-0000-000000000104", UserId: "00000000-0000-0000-0000-000000000001", SessionId: "39459de2-336d-4493-980e-97d6bf2f531b", IssuedAt: "2021-12-16T12:35:37", ExpiresAt: "2021-12-16T12:35:37", IsUsed: false, IsDeclined: false },
    { _id: "00000000-0000-0000-0000-000000000105", UserId: "00000000-0000-0000-0000-000000000001", SessionId: "ba2c0c85-f201-49d1-aae0-2f89a85bb895", IssuedAt: "2021-12-16T12:35:37", ExpiresAt: "2099-12-16T12:35:37", IsUsed: false, IsDeclined: false },
    { _id: "00000000-0000-0000-0000-000000000201", UserId: "00000000-0000-0000-0000-000000000002", SessionId: "", IssuedAt: "2021-12-16T12:35:37", ExpiresAt: "2099-12-16T12:35:37", IsUsed: false, IsDeclined: false }
];

db.refreshTokens.remove({ });
db.refreshTokens.insert(refreshTokens);