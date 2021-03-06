package com.ctfo.context;

import javax.servlet.ServletContext;
import javax.servlet.ServletContextEvent;

import org.springframework.web.context.ContextLoaderListener;


public class StartUpListener extends ContextLoaderListener {

	@Override
	public void contextInitialized(ServletContextEvent event) {

		ServletContext context = event.getServletContext();
		FrameworkContext.context = context;
		FrameworkContext.appPath = context.getRealPath("/");
		System.out.println("[框架上下文初始化成功]");
		System.out.println("程序启动目录："+FrameworkContext.appPath);
		super.contextInitialized(event);
	}


}
