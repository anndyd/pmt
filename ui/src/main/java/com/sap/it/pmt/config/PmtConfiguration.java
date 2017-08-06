package com.sap.it.pmt.config;

import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.DefaultServletHandlerConfigurer;
import org.springframework.web.servlet.config.annotation.EnableWebMvc;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurerAdapter;

@Configuration
@EnableWebMvc
@ComponentScan(basePackages = {"com.sap.it.pmt"})
public class PmtConfiguration extends WebMvcConfigurerAdapter {
	/**
     * Enable default view ("index.html") mapped under "/".
     */
    @Override
    public void configureDefaultServletHandling(DefaultServletHandlerConfigurer configurer) {
        configurer.enable();
    }
    /**
     * Set up the cached resource handling for OpenUI5 runtime served from the webjar in {@code /WEB-INF/lib} directory
     * and local JavaScript files in {@code /resources} directory.
     */
    @Override
    public void addResourceHandlers(ResourceHandlerRegistry registry) {
        registry.addResourceHandler("/resources/**").addResourceLocations("classpath:/resources/", "/resources/**")
                .setCachePeriod(31556926);
    }
}
