namespace LegendarySocialNetwork.DataClasses.Dtos
{
    public class UserDto
    {
        public required string Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public required string First_name { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public required string Second_name { get; set; }

        /// <summary>
        /// Возраст
        /// </summary>
        public required int Age { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public required string Sex { get; set; }

        /// <summary>
        /// Интересы
        /// </summary>
        public required string Biography { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        public required string City { get; set; }
    }
}
