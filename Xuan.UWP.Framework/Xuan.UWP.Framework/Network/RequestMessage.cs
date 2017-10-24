using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Xuan.UWP.Framework.Network {
    public class RequestMessage {
        public class Builder {
            private Dictionary<string, string> _headers;
            private string _url;
            private string _encoding;
            private string _body { get; set; }
            private string _method { get; set; }
            private string _contentType { get; set; }
            private string _accept { get; set; }
            private object _tag { get; set; }

            private IHttpContent _httpContent;

            public Builder() {

            }

            public Builder Url(string url) {
                _url = url;
                return this;
            }


            public Builder Method(string method) {
                _method = method;
                return this;
            }
            public Builder Body(string body) {
                _body = body;
                return this;
            }

            public Builder Header(string name, string value) {
                if (_headers == null) {
                    _headers = new Dictionary<string, string>();
                }
                _headers[name] = value;
                return this;
            }
            public Builder HttpContent(IHttpContent httpContent) {
                _httpContent = httpContent;
                return this;
            }

            public Builder Referer(string referer) {
                if (referer != null) {
                    Header("Referer", referer);
                }
                return this;
            }

            public Builder UserAgent(string userAgent) {
                if (userAgent != null) {
                    Header("User-Agent", userAgent);
                }
                return this;
            }

            public Builder ContentType(string contentType) {
                _contentType = contentType;
                return this;
            }

            public Builder Accept(string accept) {
                _accept = accept;
                return this;
            }

            public Builder Encoding(string encoding) {
                _encoding = encoding;
                Header("Encoding", encoding);
                return this;
            }

            public Builder Tag(string tag) {
                _tag = tag;
                return this;
            }


            public Builder Headers(Dictionary<string, string> headers) {
                if (_headers == null)
                    _headers = new Dictionary<string, string>(headers);
                else {
                    foreach (var header in headers) {
                        if (!_headers.ContainsKey(header.Key)) {
                            _headers.Add(header.Key, header.Value);
                        }
                    }
                }
                return this;
            }
            public HttpRequestMessage Build() {
                var message = new HttpRequestMessage();
                switch (_method) {
                    case "GET":
                    case "get":
                    case "Get":
                        message.Method = HttpMethod.Get;
                        break;
                    case "DELETE":
                    case "Delete":
                    case "delete":
                        message.Method = HttpMethod.Delete;
                        break;
                    case "POST":
                    case "Post":
                    case "post":
                        message.Method = HttpMethod.Post;
                        break;
                    case "PUT":
                    case "Put":
                    case "put":
                        message.Method = HttpMethod.Put;
                        break;
                    case "HTTPPATCH":
                    case "HttpPatch":
                    case "httppatch":
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                if (_httpContent == null)
                    message.Content = GetStringContent();
                else
                    message.Content = _httpContent;
                if (_headers != null) {
                    foreach (var header in _headers) {
                        message.Headers.Add(header.Key, header.Value);
                    }
                }
                if (!string.IsNullOrEmpty(_accept)) {
                    message.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue(_accept));
                }
                if (!string.IsNullOrEmpty(_url))
                    message.RequestUri = new Uri(_url);
                return message;
            }

            private IHttpContent GetStringContent() {
                IHttpContent content = null;
                if (!string.IsNullOrEmpty(_body)) {
                    content = new HttpStringContent(_body);
                }
                if (null != content && null != _contentType) {
                    content.Headers.ContentType = new HttpMediaTypeHeaderValue(_contentType);
                }
                return content;
            }
        }

    }
}
