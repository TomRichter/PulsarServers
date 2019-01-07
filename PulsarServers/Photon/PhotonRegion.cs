namespace PulsarServers.Photon
{
    /// <summary>
    /// Helper wrapper around PhotonClient and useful metadata for display purposes.
    /// </summary>
    class PhotonRegion
    {
        /// <summary>
        /// Photon lobby's region code (e.g., us, eu)
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Full name of the region (e.g., United States, Europe)
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// URL of icon used to visually represent the region.  Ideally visible at low resolutions.
        /// </summary>
        public string IconURL { get; }

        /// <summary>
        /// Photon client connected to this region.
        /// </summary>
        public PhotonClient PhotonClient { get; private set; } = null;

        /// <summary>
        /// PhotonRegion constructor.
        /// </summary>
        /// <param name="Code">Photon lobby's region code (e.g., us, eu)</param>
        /// <param name="Name">Full name of the region (e.g., United States, Europe)</param>
        /// <param name="IconURL">URL of icon used to visually represent the region.  Ideally visible at low resolutions.</param>
        public PhotonRegion(string Code, string Name, string IconURL)
        {
            this.Code = Code;
            this.Name = Name;
            this.IconURL = IconURL;
        }

        /// <summary>
        /// Starts regional Photon client's connection to the service.
        /// </summary>
        /// <param name="appId">Photon application ID, from game or Photon website.</param>
        /// <param name="gameVersion">Game version string.</param>
        public void StartClient(string appId, string gameVersion)
        {
            if (PhotonClient == null)
            {
                PhotonClient = new PhotonClient(appId, gameVersion, Code);
            }
        }
    }
}
