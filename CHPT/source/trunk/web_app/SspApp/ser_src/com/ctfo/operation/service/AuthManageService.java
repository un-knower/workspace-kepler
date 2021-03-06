package com.ctfo.operation.service;

import java.util.Map;

import com.ctfo.export.RemoteJavaService;
import com.ctfo.local.obj.DynamicSqlParameter;
import com.ctfo.local.obj.PaginationResult;
import com.ctfo.operation.beans.CompanyInfo;

public interface AuthManageService extends RemoteJavaService {
	/**
	 * 
	 * @description:分页时获取记录总数
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月14日下午1:55:38
	 * @modifyInformation：
	 */
	public int count(DynamicSqlParameter param);
	
	/**
	 * 
	 * @description:获取分页记录
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月14日下午5:45:02
	 * @modifyInformation：
	 */
	public PaginationResult<CompanyInfo> selectPagination(DynamicSqlParameter param);
	/**
	 * 
	 * @description:根据主键获取对象
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月16日上午11:41:18
	 * @modifyInformation：
	 */
	public CompanyInfo selectPKById(Map<String,Object> map);
	public CompanyInfo selectPKByCom(String comId);
	/**
	 * 
	 * @description:添加公司信息
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月21日下午17:15:17
	 * @modifyInformation：
	 */
	public void insert(CompanyInfo companyInfo);
	/**
	 * 
	 * @description:吊销功能
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月27日上午16:05:44
	 * @modifyInformation：
	 */
	public void updateRevokeOpen(Map<String,Object> map);
	/**
	 * 
	 * @description:审批
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月27日上午16:05:44
	 * @modifyInformation：
	 */
	public int updateAuthApproval(Map<String,String> map);
	/**
	 * 
	 * @description:管理
	 * @param:
	 * @author: 马驰
	 * @creatTime:  2014年10月29日上午10:33:44
	 * @modifyInformation：
	 */
	public int updateAuthManage(Map<String,String> map);
	/**cs端调用企业注册接口
	 * @param map
	 * @return
	 */
	public Map<String, String> remoteRegisterAuth(String json,String remoteIp);
	/**
	 * cs端调用企业注册结果
	 * @param json
	 * @return
	 */
	public Map<String, String> registerAuthResult(String remoteMachineCode);
	/**
	 * cs端获取软件使用有效期
	 * @param json
	 * @return
	 */
	public Map<String, String> getValidate(String remoteComId);	
	
	public CompanyInfo selectPKByMachine(String MachineNo);
}
