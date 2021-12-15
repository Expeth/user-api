// password is 123456aF1!
var users = [
    { _id: "00000000-0000-0000-0000-000000000001", Username: "testuser1", FirstName: "test1", LastName: "test1", MiddleName: "test1", Email: "testuser1@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" },
    { _id: "00000000-0000-0000-0000-000000000002", Username: "testuser2", FirstName: "test2", LastName: "test2", MiddleName: "test2", Email: "testuser2@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" },
    { _id: "00000000-0000-0000-0000-000000000003", Username: "testuser3", FirstName: "test3", LastName: "test3", MiddleName: "test3", Email: "testuser3@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" },
    { _id: "00000000-0000-0000-0000-000000000004", Username: "testuser4", FirstName: "test4", LastName: "test4", MiddleName: "test4", Email: "testuser4@emample.com", PasswordHash: "sD0UuzNtOS0EQp6PRocefnYNavsYtdmt9/AYFinCNS8OaYzsyMzJ+G+53REqDtCrkojdoT68TV8tnd2qeX4wNw==", PasswordSalt: "AmYei2/Ns1XIk9+uDpewzg==", PasswordHashingAlgorithm: "0", ProfilePictureUri: "" }
];

db.users.insert(users);