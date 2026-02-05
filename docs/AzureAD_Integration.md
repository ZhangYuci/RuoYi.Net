# Azure AD 登录集成说明

本文档说明如何在 RuoYi.Net 项目中配置和使用 Azure Active Directory (AAD) 登录功能。

## 功能概述

此功能允许用户通过 Microsoft Azure Active Directory 进行身份验证登录，支持企业级单点登录 (SSO)。

## 前置条件

1. 拥有 Azure Active Directory 租户
2. 在 Azure Portal 中注册应用程序
3. 系统中已存在对应的用户账号

## Azure Portal 配置步骤

### 1. 注册应用程序

1. 登录 [Azure Portal](https://portal.azure.com)
2. 导航到 **Azure Active Directory** > **应用注册** > **新注册**
3. 输入应用程序名称（例如：RuoYi.Net）
4. 选择支持的账户类型
5. 配置重定向 URI：
   - 平台：Web
   - URI：`https://your-domain.com/signin-oidc`（根据实际域名修改）
6. 点击**注册**

### 2. 配置应用程序

1. 在应用程序概述页面，记录以下信息：
   - **应用程序(客户端) ID**
   - **目录(租户) ID**
   - **域名**（例如：contoso.onmicrosoft.com）

2. 创建客户端密码：
   - 导航到 **证书和密码** > **新客户端密码**
   - 输入描述，选择过期时间
   - 点击**添加**
   - **立即复制并保存密码值**（只显示一次）

3. 配置 API 权限：
   - 导航到 **API 权限**
   - 确保包含以下权限：
     - Microsoft Graph > User.Read (Delegated)
   - 点击**授予管理员同意**（如果需要）

## RuoYi.Net 配置步骤

### 1. 修改 appsettings.json

在 `RuoYi.Admin/appsettings.json` 文件中配置 Azure AD 信息：

```json
{
  "AzureAd": {
    "Enabled": true,
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "contoso.onmicrosoft.com",
    "TenantId": "your-tenant-id-here",
    "ClientId": "your-client-id-here",
    "ClientSecret": "your-client-secret-here",
    "CallbackPath": "/signin-oidc"
  }
}
```

**配置说明：**
- `Enabled`: 设置为 `true` 启用 Azure AD 登录
- `Instance`: Azure AD 实例地址（默认值通常不需要修改）
- `Domain`: Azure AD 域名
- `TenantId`: 从 Azure Portal 获取的租户 ID
- `ClientId`: 从 Azure Portal 获取的应用程序(客户端) ID
- `ClientSecret`: 从 Azure Portal 获取的客户端密码
- `CallbackPath`: 回调路径（默认值通常不需要修改）

### 2. 创建系统用户

在使用 Azure AD 登录之前，需要在 RuoYi.Net 系统中创建对应的用户账号：

1. 使用管理员账号登录系统
2. 导航到 **系统管理** > **用户管理**
3. 创建新用户，**用户名必须与 Azure AD 用户的 UPN（用户主体名称）或 preferred_username 一致**
4. 配置用户的角色、部门等信息

**重要提示：** AAD 登录不会自动创建用户，必须先在系统中手动创建用户账号。

## 使用 Azure AD 登录

### 方式一：通过 API 端点

1. **发起 AAD 登录请求**
   ```
   GET /login/aad
   ```
   此端点会重定向到 Microsoft 登录页面

2. **用户完成认证后，系统会自动回调**
   ```
   GET /login/aad/callback
   ```
   成功后返回 JWT token：
   ```json
   {
     "code": 200,
     "msg": "Azure AD 登录成功",
     "token": "eyJhbGciOiJIUzI1NiIs..."
   }
   ```

### 方式二：通过前端集成

前端可以在登录页面添加"使用 Microsoft 登录"按钮：

```javascript
// 发起 AAD 登录
function loginWithAAD() {
  window.location.href = '/login/aad';
}
```

## API 端点

### GET /login/aad
发起 Azure AD 登录流程

**响应：** 重定向到 Microsoft 登录页面

### GET /login/aad/callback
Azure AD 登录回调端点（自动处理）

**成功响应：**
```json
{
  "code": 200,
  "msg": "Azure AD 登录成功",
  "token": "jwt-token-here"
}
```

**错误响应：**
```json
{
  "code": 500,
  "msg": "登录失败: 错误详情"
}
```

## 安全注意事项

1. **保护客户端密码：** 
   - 不要将 `ClientSecret` 提交到版本控制系统
   - 使用环境变量或密钥管理服务存储敏感信息
   - 定期轮换客户端密码

2. **HTTPS 要求：**
   - 生产环境必须使用 HTTPS
   - 确保回调 URL 使用 HTTPS 协议

3. **用户权限管理：**
   - AAD 登录的用户仍然遵循系统的角色和权限设置
   - 定期审查用户权限

4. **日志记录：**
   - 系统会记录 AAD 登录成功和失败的日志
   - 可在 **系统监控** > **登录日志** 中查看

## 故障排查

### 问题 1：重定向 URI 不匹配
**错误信息：** "The reply URL specified in the request does not match..."

**解决方案：**
- 确认 Azure Portal 中配置的重定向 URI 与实际应用地址一致
- 检查 `appsettings.json` 中的 `CallbackPath` 配置

### 问题 2：用户不存在
**错误信息：** "用户 xxx 不存在，请联系管理员创建用户"

**解决方案：**
- 在系统中创建对应的用户账号
- 确保用户名与 Azure AD 的 UPN 或 preferred_username 一致

### 问题 3：认证失败
**错误信息：** "Azure AD 认证失败"

**解决方案：**
- 检查 TenantId、ClientId、ClientSecret 是否正确
- 确认应用程序权限配置正确
- 查看应用程序日志获取详细错误信息

### 问题 4：用户被停用或删除
**错误信息：** "用户已被停用/删除"

**解决方案：**
- 在 **用户管理** 中启用用户账号
- 确认用户状态为正常

## 配置示例

### 开发环境配置
```json
{
  "AzureAd": {
    "Enabled": true,
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "contoso.onmicrosoft.com",
    "TenantId": "12345678-1234-1234-1234-123456789012",
    "ClientId": "87654321-4321-4321-4321-210987654321",
    "ClientSecret": "your-dev-secret",
    "CallbackPath": "/signin-oidc"
  }
}
```

### 生产环境配置（使用环境变量）
```json
{
  "AzureAd": {
    "Enabled": true,
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "${AZURE_AD_DOMAIN}",
    "TenantId": "${AZURE_AD_TENANT_ID}",
    "ClientId": "${AZURE_AD_CLIENT_ID}",
    "ClientSecret": "${AZURE_AD_CLIENT_SECRET}",
    "CallbackPath": "/signin-oidc"
  }
}
```

## 相关资源

- [Microsoft Identity Platform 文档](https://learn.microsoft.com/azure/active-directory/develop/)
- [Azure AD 应用注册指南](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app)
- [OpenID Connect 协议](https://learn.microsoft.com/azure/active-directory/develop/v2-protocols-oidc)

## 技术支持

如有问题，请联系系统管理员或查看项目 GitHub Issues。
