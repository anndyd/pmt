package com.sap.it.pmt.util;

import java.io.IOException;
import java.security.KeyManagementException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;

import javax.naming.Context;
import javax.naming.InitialContext;
import javax.naming.NamingException;
import javax.net.ssl.SSLContext;

import org.apache.http.HttpEntity;
import org.apache.http.HttpHeaders;
import org.apache.http.HttpHost;
import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpRequestBase;
import org.apache.http.entity.ByteArrayEntity;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.ssl.SSLContextBuilder;
import org.apache.http.util.EntityUtils;
import org.springframework.http.HttpMethod;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.sap.cloud.account.TenantContext;
import com.sap.core.connectivity.api.configuration.ConnectivityConfiguration;
import com.sap.core.connectivity.api.configuration.DestinationConfiguration;

public class RestClientHelper {
    private TenantContext tenantContext;

    private final String DESTINATION = "pekitwin2008vm";
	private final String ON_PREMISE_PROXY = "OnPremise";
	
    private final String TRUSTSTORE = "trust.keystore";
    private final String STORE_PASSWORD = "changeit";

	@SuppressWarnings("unchecked")
	public <T, R> R execute(String targetPath, HttpMethod httpMethod, 
			T payload, Class<R> responseType) {
		R rlt = null;
		ObjectMapper om = new ObjectMapper();
		try {
			// Allow TLSv1 protocol only
//			SSLConnectionSocketFactory sslsf = new SSLConnectionSocketFactory(
//					getSSLContext(),
//			        new String[] { "TLSv1" },
//			        null,
//			        SSLConnectionSocketFactory.getDefaultHostnameVerifier());
//			CredentialsProvider credentialsProvider = new BasicCredentialsProvider();
//			credentialsProvider.setCredentials(AuthScope.ANY, creds);
			CloseableHttpClient httpclient = HttpClients.custom()
//					.setDefaultCredentialsProvider(credentialsProvider)
//			        .setSSLSocketFactory(sslsf)
			        .build();
			try {
				byte[] data = null;
				if (null != payload) {
					data = om.writeValueAsBytes(payload);
				}
				HttpRequestBase request = generateRequestFromDestination(targetPath, httpMethod, data);
				
			    CloseableHttpResponse response = httpclient.execute(request);
			    try {
			        HttpEntity entity = response.getEntity();
			        String res = EntityUtils.toString(entity);
			        if (String.class == responseType) {
			        	rlt = (R) res;
			        } else {
			        	rlt = om.readValue(res, responseType);
			        }
			    } finally {
			        response.close();
			    }
			} finally {
			    httpclient.close();
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
        return rlt;
	}

	private SSLContext getSSLContext() throws KeyStoreException, NoSuchAlgorithmException, CertificateException,
			IOException, KeyManagementException {
		KeyStore trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
		trustStore.load(getClass().getResourceAsStream(TRUSTSTORE), STORE_PASSWORD.toCharArray());
		return new SSLContextBuilder().loadTrustMaterial(null, (X509Certificate[] arg0, String arg1) -> {
			return true;
		}).build();
	}

	private HttpRequestBase generateRequestFromDestination(String targetPath, HttpMethod httpMethod, byte[] data) throws Exception {
        // Look up the connectivity configuration API
        Context ctx = new InitialContext();
        ConnectivityConfiguration configuration =
                (ConnectivityConfiguration) ctx.lookup("java:comp/env/connectivityConfiguration");
        // Get destination configuration for "destinationName"
        DestinationConfiguration destConfiguration = configuration.getConfiguration(DESTINATION);
        if (destConfiguration == null) {
        	throw new Exception(String.format("Destination %s is not found. Hint:"
                            + " Make sure to have the destination configured.", DESTINATION));
        }
        // Get the destination URL, proxy
        String value = destConfiguration.getProperty("URL");
        String url = value + "/" + targetPath;
        
        HttpRequestBase request = new HttpGet(url);
        if (HttpMethod.POST.equals(httpMethod)) {
        	request = new HttpPost(url);
        	((HttpPost) request).setEntity(new ByteArrayEntity(data));
        }
        
        String proxyType = destConfiguration.getProperty("ProxyType");
        HttpHost proxy = getProxy(proxyType);
        
        RequestConfig config = RequestConfig.custom().setProxy(proxy).build();
        request.setConfig(config);
        injectHeader(request, proxyType);
        
        return request;
	}

    private void injectHeader(HttpRequestBase request, String proxyType) throws NamingException {
        request.addHeader(HttpHeaders.CONTENT_TYPE, "application/json");
        if (ON_PREMISE_PROXY.equals(proxyType)) {
        	Context ctx = new InitialContext();
        	tenantContext = (TenantContext) ctx.lookup("java:comp/env/TenantContext");
            // Insert header for on-premise connectivity with the consumer account name
            request.addHeader("SAP-Connectivity-ConsumerAccount",
                    tenantContext.getTenant().getAccount().getId());
        }
    }
	
    private HttpHost getProxy(String proxyType) {
        String proxyHost = null;
        int proxyPort;

        if (ON_PREMISE_PROXY.equals(proxyType)) {
            // Get proxy for on-premise destinations
            proxyHost = System.getenv("HC_OP_HTTP_PROXY_HOST");
            proxyPort = Integer.parseInt(System.getenv("HC_OP_HTTP_PROXY_PORT"));
        } else {
            // Get proxy for internet destinations
            proxyHost = System.getProperty("https.proxyHost");
            proxyPort = Integer.parseInt(System.getProperty("https.proxyPort"));
        }

        return new HttpHost(proxyHost, proxyPort);
    }

}
