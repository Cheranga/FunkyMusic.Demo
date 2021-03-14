﻿using System.ComponentModel;

namespace FunkyMusic.Demo.Infrastructure.Api.Dto.Assets
{
    public class MusicArtistDto
    {
        public string Id { get; set; }
        [DefaultValue(0)]
        public int Score { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Disambiguation { get; set; }
    }
}