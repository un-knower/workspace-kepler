package com.ctfo.dao.impl;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.ctfo.beans.Operator;
import com.ctfo.beans.Role;
import com.ctfo.dao.RoleManageDAO;
import com.ctfo.exception.CtfoAppException;
import com.ctfo.service.DynamicSqlParameter;

/**
 * 
 * 
 * <p>
 * -----------------------------------------------------------------------------
 * <br>
 * 工程名 ： InformationSer <br>
 * 功能： <br>
 * 描述： <br>
 * 授权 : (C) Copyright (c) 2011 <br>
 * 公司 : 北京中交兴路信息科技有限公司 <br>
 * -----------------------------------------------------------------------------
 * <br>
 * 修改历史 <br>
 * <table width="432" border="1">
 * <tr>
 * <td>版本</td>
 * <td>时间</td>
 * <td>作者</td>
 * <td>改变</td>
 * </tr>
 * <tr>
 * <td>1.0</td>
 * <td>2012-12-12</td>
 * <td>xuehui</td>
 * <td>创建</td>
 * </tr>
 * </table>
 * <br>
 * <font color="#FF0000">注意: 本内容仅限于[北京中交兴路信息科技有限公司]内部使用，禁止转发</font> <br>
 * 
 * @version 1.0
 * 
 * @author xuehui
 * @since JDK1.6
 */
@SuppressWarnings("unchecked")
public class RoleManageDAOImpl extends GenericIbatisAbstract<Role, Long> implements RoleManageDAO {

	/***
	 * 根据条件查询登陆信息
	 */
	public static final String GET_ROLE_LIST = "getRoleList";
	public static final String GET_ROLE_LIST_COUNT = "getRoleListCount";
	private static final String GET_EDIT_ROLE = "getEditRole";
	private static final String ADD_ROLE = "addRole";
	private static final String DEL_ROLE = "delRole";
	private static final String EDIT_ROLE = "editRole";
	private static final String CHECK_ROLE_EXIST = "checkRoleExist";

	//@Override
	public Map<String,Object> getRoleList(DynamicSqlParameter param) throws Exception {
		Map<String,Object> map = new HashMap<String,Object>();
		try {
			List<Role> data = getSqlMapClientTemplate().queryForList(sqlmapNamespace + "." + GET_ROLE_LIST, param);
			int total = (Integer) getSqlMapClientTemplate().queryForObject(sqlmapNamespace + "." + GET_ROLE_LIST_COUNT, param);
			map.put("data", data);
			map.put("total", total);
		} catch (Exception e) {
			map = null;
			throw new CtfoAppException(e.fillInStackTrace());
		}
		return map;
	}

	//@Override
	public Map<String, Object> getEditRole(DynamicSqlParameter requestParam) {
		Map<String,Object> map = new HashMap<String,Object>();
		try {
			List<Operator> data = getSqlMapClientTemplate().queryForList(sqlmapNamespace + "." + GET_EDIT_ROLE, requestParam);
			map.put("data", data);
		} catch (Exception e) {
			map = null;
			throw new CtfoAppException(e.fillInStackTrace());
		}
		return map;
	}

	//@Override
	public int addRole(DynamicSqlParameter requestParam) {
		int isertRoleId = 0;
		try {
			isertRoleId = getSqlMapClientTemplate().update(sqlmapNamespace + "." + ADD_ROLE, requestParam);
		} catch (Exception e) {
			throw new CtfoAppException(e.fillInStackTrace());
		}
		return isertRoleId;
	}

	//@Override
	public int delRole(DynamicSqlParameter requestParam) {
		int delRoleId = 0;
		try {
			delRoleId = getSqlMapClientTemplate().update(sqlmapNamespace + "." + DEL_ROLE, requestParam);
		} catch (Exception e) {
			throw new CtfoAppException(e.fillInStackTrace());
		}
		return delRoleId;
	}

	//@Override
	public int editRole(DynamicSqlParameter requestParam) {
		int editRoleId = 0;
		try {
			editRoleId = getSqlMapClientTemplate().update(sqlmapNamespace + "." + EDIT_ROLE, requestParam);
		} catch (Exception e) {
			throw new CtfoAppException(e.fillInStackTrace());
		}
		return editRoleId;
	}

	public int checkRoleExist(DynamicSqlParameter requestParam) {
		int roleExist = 0;
		try {
			roleExist = (Integer) getSqlMapClientTemplate().queryForObject(sqlmapNamespace + "." + CHECK_ROLE_EXIST, requestParam);
		} catch (Exception e) {
			throw new CtfoAppException(e.fillInStackTrace());
		}
		return roleExist;
	}

}
