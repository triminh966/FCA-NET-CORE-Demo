using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FCA.Core.Secrets
{
    /// <summary>
    /// <see cref="AbstractSecretManager"/> interface.
    /// </summary>
    public interface ISecretManager
    {
        /// <summary>
        /// Refresh method to refresh secret store.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Retrieves secret's value from secret store.
        /// </summary>
        /// <param name="secretKey">Secret's key.</param>
        /// <value>"Secret String"</value>
        /// <returns>Secret string.</returns>
        string RetrieveSecretValue(string secretKey);
    }

    /// <summary>
    /// An implementation of <see cref="ISecretManager"/>.
    /// </summary>
    public sealed class AbstractSecretManager : ISecretManager
    {
        #region Private Members

        private Dictionary<string, string> _secretDictionary;

        private readonly string _secretName;
        private readonly string _region;
        private readonly string _accessKey;
        private readonly string _secretKey;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates <see cref="AbstractSecretManager"/>.
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>
        public AbstractSecretManager(IConfiguration configuration)
        {
            _secretDictionary = new Dictionary<string, string>();
            _secretName = configuration["SecretName"];
            _region = configuration["SecretRegion"];
            _accessKey = configuration["AWS:AccessKey"];
            _secretKey = configuration["AWS:SecretKey"];


            Initialise();
        }

        #endregion

        #region Class Methods

        /// <summary>
        /// Re-Initialise SecretDictionary when refresh.
        /// </summary>
        public void Refresh()
        {
            Initialise();
        }

        /// <summary>
        /// Retrieves secret value.
        /// </summary>
        /// <param name="secretKey">SecretKey.</param>
        /// <returns>SecretValue if secret key is found,
        /// Otherwise, empty string.</returns>
        public string RetrieveSecretValue(string secretKey)
        {
            return _secretDictionary.ContainsKey(secretKey) ?
                _secretDictionary[secretKey] :
                string.Empty;
        }

        private void Initialise()
        {
            try
            {
                IAmazonSecretsManager client = new AmazonSecretsManagerClient(_accessKey, _secretKey, RegionEndpoint.GetBySystemName(_region));
                var request = new GetSecretValueRequest()
                {
                    SecretId = _secretName,
                    VersionStage = "AWSCURRENT"
                };
                
                var response = client.GetSecretValueAsync(request).Result;

                if (response.SecretString != null)
                {
                    var secret = response.SecretString;
                    _secretDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(secret);
                }
                else
                {
                    var memoryStream = response.SecretBinary;
                    var reader = new StreamReader(memoryStream);
                    var decodedBinarySecret = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                    _secretDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedBinarySecret);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
