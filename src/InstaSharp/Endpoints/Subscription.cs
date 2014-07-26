﻿using System.Collections.Generic;
using InstaSharp.Extensions;
using InstaSharp.Models.Responses;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace InstaSharp.Endpoints
{
    public class Subscription
    {
        public enum Object
        {
            User,
            Tag,
            Location,
            Geography
        }

        public enum Aspect
        {
            Media
        }

        private readonly InstagramConfig config;
        private readonly HttpClient client;

        public Subscription(InstagramConfig config)
        {
            this.config = config;
            client = new HttpClient { BaseAddress = new Uri(config.RealTimeApi) };
        }

        /// <summary>
        /// Create a subscription
        /// </summary>
        /// <param name="type"></param>
        /// <param name="aspect"></param>
        /// <param name="objectId">This is required, i.e. if <see cref="type"/> is<see cref="Object.Tag"/> or <see cref="Object.Location"/></param>
        /// <param name="verifyToken"></param>
        /// <returns></returns>
        public Task<SubscriptionsResponse> Create(Object type, Aspect aspect, String objectId = null, String verifyToken = null)
        {
            // create a new guid that uniquely identifies this subscription request
            verifyToken = String.IsNullOrWhiteSpace(verifyToken) ? Guid.NewGuid().ToString() : verifyToken;
            var postParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", config.ClientId),
                new KeyValuePair<string, string>("client_secret", config.ClientSecret),
                new KeyValuePair<string, string>("object", type.ToString().ToLower()),
                new KeyValuePair<string, string>("aspect", aspect.ToString().ToLower()),
                new KeyValuePair<string, string>("verify_token", verifyToken),
                new KeyValuePair<string, string>("callback_url", config.CallbackUri)
            };

            if ((type == Object.Tag || type == Object.Location) && objectId != null)
            {
                postParams.Add(new KeyValuePair<string, string>("object_id", objectId.ToLower()));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
            {
                Content = new FormUrlEncodedContent(postParams)
            };

            return client.ExecuteAsyncWithMeta<SubscriptionsResponse>(request);
        }

        /// <summary>
        /// Deletes a subscription by subscription id
        /// </summary>
        /// <param name="id">The subscription id</param>
        /// <returns></returns>
        public Task<SubscriptionsResponse> UnsubscribeUser(string id)
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = client.BaseAddress };

            request.AddParameter("client_id", config.ClientId);
            request.AddParameter("client_secret", config.ClientSecret);
            request.AddParameter("id", id);

            return client.ExecuteAsyncWithMeta<SubscriptionsResponse>(request);
        }

        /// <summary>
        /// Deletes a subscription by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<SubscriptionsResponse> RemoveSubscription(Object type)
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = client.BaseAddress };

            request.AddParameter("client_id", config.ClientId);
            request.AddParameter("client_secret", config.ClientSecret);
            request.AddParameter("object", type.ToString().ToLower());

            return client.ExecuteAsyncWithMeta<SubscriptionsResponse>(request);
        }

        /// <summary>
        /// Removes all subscriptions
        /// </summary>
        /// <returns></returns>
        public Task<SubscriptionsResponse> RemoveAllSubscriptions()
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = client.BaseAddress };

            request.AddParameter("client_id", config.ClientId);
            request.AddParameter("client_secret", config.ClientSecret);
            request.AddParameter("object", "all");

            return client.ExecuteAsyncWithMeta<SubscriptionsResponse>(request);
        }

        /// <summary>
        /// Lists all subscriptions
        /// </summary>
        /// <returns></returns>
        public Task<SubscriptionsResponse> ListAllSubscriptions()
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = client.BaseAddress };

            request.AddParameter("client_id", config.ClientId);
            request.AddParameter("client_secret", config.ClientSecret);

            return client.ExecuteAsyncWithMeta<SubscriptionsResponse>(request);
        }
    }
}
