using Domain.Post.ValueObjects;
using Domain.Profile;
using Domain.Profile.ValueObjects;

namespace UnitTests.Domain
{
    public class ProfileShould
    {
        private readonly string _validLogin = "testuser";
        private readonly string _validFirstName = "John";
        private readonly string _validLastName = "Doe";
        private readonly string _validPatronymic = "Smith";
        private readonly Guid _validFollowerId = Guid.NewGuid();
        private readonly Guid _validSubscriberId = Guid.NewGuid();

        [Fact]
        public void Create_WithValidParameters_ReturnsProfile()
        {
            // Act
            var result = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic);

            // Assert
            Assert.True(result.IsSuccess);
            var profile = result.Value;
            Assert.Equal(_validLogin, profile.Login);
            Assert.Equal(_validFirstName, profile.Names.First);
            Assert.Equal(_validLastName, profile.Names.Second);
            Assert.Equal(_validPatronymic, profile.Names.Patronymic);
            Assert.NotEqual(Guid.Empty, profile.Id);
            Assert.NotEqual(default(DateTime), profile.CreatedAtUtc);
            Assert.Equal(Availability.Public, profile.AvailabilityLevel);
            Assert.Empty(profile.Subscribers);
            Assert.Empty(profile.Followers);
        }

        [Fact]
        public void Create_WithEmptyLogin_ReturnsFailure()
        {
            // Act
            var result = Profile.Create("", _validFirstName, _validLastName, _validPatronymic);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains(nameof(Profile.Login)));
        }

        [Fact]
        public void Create_WithEmptyFirstName_ReturnsFailure()
        {
            // Act
            var result = Profile.Create(_validLogin, "", _validLastName, _validPatronymic);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains("FirstName"));
        }

        [Fact]
        public void Create_WithEmptyLastName_ReturnsFailure()
        {
            // Act
            var result = Profile.Create(_validLogin, _validFirstName, "", _validPatronymic);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains("SecondName"));
        }

        [Fact]
        public void Create_WithNullPatronymic_Succeeds()
        {
            // Act
            var result = Profile.Create(_validLogin, _validFirstName, _validLastName, null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value.Names.Patronymic);
        }

        [Fact]
        public void AddFollower_WithNewFollower_AddsToFollowers()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;

            // Act
            var result = profile.AddFollower(_validFollowerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(profile.Followers);
            Assert.Equal(_validFollowerId, profile.Followers.First());
        }

        [Fact]
        public void AddFollower_WithExistingFollower_ReturnsFailure()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;
            profile.AddFollower(_validFollowerId);

            // Act
            var result = profile.AddFollower(_validFollowerId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e is AlreadyFollowedError);
            Assert.Single(profile.Followers);
        }

        [Fact]
        public void RemoveFollower_WithExistingFollower_RemovesFromFollowers()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;
            profile.AddFollower(_validFollowerId);

            // Act
            var result = profile.RemoveFollower(_validFollowerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(profile.Followers);
        }

        [Fact]
        public void RemoveFollower_WithNonExistentFollower_ReturnsFailure()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;

            // Act
            var result = profile.RemoveFollower(_validFollowerId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e is NotFollowedError);
            Assert.Empty(profile.Followers);
        }

        [Fact]
        public void AddSubscriber_WithNewSubscriber_AddsToSubscribers()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;

            // Act
            var result = profile.AddSubscriber(_validSubscriberId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(profile.Subscribers);
            Assert.Equal(_validSubscriberId, profile.Subscribers.First());
        }

        [Fact]
        public void AddSubscriber_WithExistingSubscriber_ReturnsFailure()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;
            profile.AddSubscriber(_validSubscriberId);

            // Act
            var result = profile.AddSubscriber(_validSubscriberId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e is AlreadySubscribedError);
            Assert.Single(profile.Subscribers);
        }

        [Fact]
        public void RemoveSubscriber_WithExistingSubscriber_RemovesFromSubscribers()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;
            profile.AddSubscriber(_validSubscriberId);

            // Act
            var result = profile.RemoveSubscriber(_validSubscriberId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(profile.Subscribers);
        }

        [Fact]
        public void RemoveSubscriber_WithNonExistentSubscriber_ReturnsFailure()
        {
            // Arrange
            var profile = Profile.Create(_validLogin, _validFirstName, _validLastName, _validPatronymic).Value;

            // Act
            var result = profile.RemoveSubscriber(_validSubscriberId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e is NotSubscribedError);
            Assert.Empty(profile.Subscribers);
        }

        [Fact]
        public void Name_Create_WithValidParameters_ReturnsName()
        {
            // Act
            var result = Name.Create(_validFirstName, _validLastName, _validPatronymic);

            // Assert
            Assert.True(result.IsSuccess);
            var name = result.Value;
            Assert.Equal(_validFirstName, name.First);
            Assert.Equal(_validLastName, name.Second);
            Assert.Equal(_validPatronymic, name.Patronymic);
        }

        [Fact]
        public void Name_Create_WithEmptyFirstName_ReturnsFailure()
        {
            // Act
            var result = Name.Create("", _validLastName, _validPatronymic);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains("FirstName"));
        }

        [Fact]
        public void Name_Create_WithEmptyLastName_ReturnsFailure()
        {
            // Act
            var result = Name.Create(_validFirstName, "", _validPatronymic);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message.Contains("SecondName"));
        }

        [Fact]
        public void Name_Create_WithNullPatronymic_Succeeds()
        {
            // Act
            var result = Name.Create(_validFirstName, _validLastName, null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value.Patronymic);
        }

        [Fact]
        public void Name_Create_TrimsWhitespace()
        {
            // Arrange
            var firstNameWithWhitespace = "  John  ";
            var lastNameWithWhitespace = "  Doe  ";
            var patronymicWithWhitespace = "  Smith  ";

            // Act
            var result = Name.Create(firstNameWithWhitespace, lastNameWithWhitespace, patronymicWithWhitespace);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(_validFirstName, result.Value.First);
            Assert.Equal(_validLastName, result.Value.Second);
            Assert.Equal(_validPatronymic, result.Value.Patronymic);
        }
    }
}