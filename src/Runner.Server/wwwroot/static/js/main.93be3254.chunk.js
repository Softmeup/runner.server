(this["webpackJsonpreact-master-detail"]=this["webpackJsonpreact-master-detail"]||[]).push([[0],{20:function(e,t,n){e.exports={component:"MasterDetail_component__Uz_TT",master:"MasterDetail_master__25jd3",detail:"MasterDetail_detail__1_NEc"}},22:function(e,t,n){e.exports={header:"Header_header__Nww_e",back:"Header_back__2pYLZ"}},25:function(e,t,n){e.exports={component:"ListItem_component__1tRbh",inner:"ListItem_inner__3kPOv"}},26:function(e,t,n){e.exports={component:"ListItemLink_component__5N6-o",active:"ListItemLink_active__14SJk"}},40:function(e,t,n){},55:function(e,t,n){"use strict";n.r(t);var r=n(0),a=n.n(r),i=n(32),s=n.n(i),c=(n(40),n(10)),o=n(4),l=n(19),u="(max-width: 599px)",d=n(22),b=n.n(d),p=n(1),j=function(e){return Object(p.jsxs)("div",{className:b.a.header,children:[Object(p.jsx)(l.a,{query:u,children:function(t){return t?Object(p.jsx)(c.b,{to:"../../",className:b.a.back,style:{visibility:e.hideBackButton?"hidden":"visible"},children:"Back"}):Object(p.jsx)("div",{children:"\xa0"})}}),Object(p.jsx)("h1",{"data-test":"HeaderTitle",children:e.title||"No Title"})]})};j.defaultProps={hideBackButton:!1};var f=n(25),m=n.n(f),h="No Data",x=function(e){return Object(p.jsx)("div",{className:m.a.component,children:Object(p.jsxs)("div",{className:m.a.inner,children:[Object(p.jsx)("h1",{"data-test":"ListItemHeading",children:e.item.title?e.item.title:h}),Object(p.jsx)("h2",{"data-test":"ListItemSubHeading",children:e.item.description?e.item.description:h})]})})},O=n(15),g=n(26),_=n.n(g),v=function(e){return Object(p.jsx)(c.c,{exact:!0,to:e.to,className:_.a.component,activeClassName:_.a.active,children:Object(p.jsx)(x,Object(O.a)({},e))})},w=n(20),k=n.n(w),N=function(e){var t=Object(o.h)().path,n=Object(p.jsx)(e.MasterType,Object(O.a)(Object(O.a)({},e.masterProps),{},{"data-test":"Master"})),r=Object(p.jsx)(e.DetailType,Object(O.a)(Object(O.a)({},e.detailProps),{},{"data-test":"Detail"}));return Object(p.jsx)(l.a,{query:u,children:function(e){return e?Object(p.jsxs)(o.d,{children:[Object(p.jsx)(o.b,{exact:!0,path:"".concat(t),children:n}),Object(p.jsx)(o.b,{path:"".concat(t,"/detail/:id"),children:r})]}):Object(p.jsxs)("section",{className:k.a.component,children:[Object(p.jsx)("section",{className:k.a.master,children:Object(p.jsx)(o.b,{path:"".concat(t),children:n})}),Object(p.jsx)("section",{className:k.a.detail,children:Object(p.jsxs)(o.d,{children:[Object(p.jsx)(o.b,{exact:!0,path:"".concat(t),children:r}),Object(p.jsx)(o.b,{path:"".concat(t,"/detail/:id"),children:r})]})})]})}})},L=n(14),I=n(6),C=n.n(I),y=n(12),S=n(11),D=function(e,t){var n="string"===typeof t?parseInt(t,10):t;if(void 0!==n&&null!==n){var r=e.find((function(e,t,r){return e.RequestId===n}))||null;if(null!==r)return{item:{id:r.RequestId,title:r.JobId,description:r.TimeLineId},job:r}}return{item:null,job:null}},T=n(9),M=n.n(T),R=n(27),q=n.n(R),E=n(35),H=n.n(E),J="http://localhost";function B(e){return P.apply(this,arguments)}function P(){return(P=Object(S.a)(C.a.mark((function e(t){var n,r,a;return C.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return n=J+"/runner/host/_apis/pipelines/workflows/"+t+"/artifacts",e.next=3,fetch(n);case 3:return r=e.sent,e.next=6,r.text();case 6:return a=e.sent,e.abrupt("return",JSON.parse(a));case 8:case"end":return e.stop()}}),e)})))).apply(this,arguments)}function W(e,t){return F.apply(this,arguments)}function F(){return(F=Object(S.a)(C.a.mark((function e(t,n){var r,a,i;return C.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return(r=new URL(n)).searchParams.append("itemPath",t),e.next=4,fetch(r.toString());case 4:return a=e.sent,e.next=7,a.text();case 7:return i=e.sent,e.abrupt("return",JSON.parse(i));case 9:case"end":return e.stop()}}),e)})))).apply(this,arguments)}var U=function(e){var t=Object(r.useState)([]),n=Object(y.a)(t,2),a=n[0],i=n[1],s=Object(r.useState)([]),c=Object(y.a)(s,2),l=c[0],u=c[1],d=Object(r.useState)([]),b=Object(y.a)(d,2),f=b[0],m=b[1],h=Object(r.useState)("Loading..."),x=Object(y.a)(h,2),O=x[0],g=x[1],_=Object(o.g)().id,v=Object(o.g)(),w=v.owner,k=v.repo,N=Object(r.useState)([]),I=Object(y.a)(N,2),T=I[0],R=I[1];return Object(r.useEffect)((function(){Object(S.a)(C.a.mark((function e(){var t,n,r,s,c,o,l,d,b,p,j;return C.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:if(m((function(e){return[]})),void 0!==_){e.next=3;break}return e.abrupt("return");case 3:if(n=Number.parseInt(_),0!==a.length&&null!=a.find((function(e){return e.RequestId===n}))){e.next=11;break}return e.next=7,fetch(J+"/"+w+"/"+k+"/_apis/v1/Message",{});case 7:return e.next=9,e.sent.json();case 9:t=e.sent,i(t);case 11:if(null!==(r=D(t||a,_)).job.errors&&r.job.errors.length>0?R(r.job.errors):R([]),s=r.item,null==(c=s?s.description:null)){e.next=29;break}return e.next=18,fetch(J+"/"+w+"/"+k+"/_apis/v1/Timeline/"+c,{});case 18:if(200!==(o=e.sent).status){e.next=27;break}return e.next=22,o.json();case 22:null!=(l=e.sent)&&l.length>1&&(g(l.shift().name),u(l)),u(l),e.next=29;break;case 27:g(null!==r.job.errors&&r.job.errors.length>0?"Failed to run":"Wait for workflow to run..."),u((function(e){return[]}));case 29:if(-1===r.job.runid){e.next=45;break}return e.next=32,B(r.job.runid);case 32:if(void 0===(d=e.sent).value){e.next=45;break}b=0;case 35:if(!(b<d.count)){e.next=44;break}return p=d.value[b],e.next=39,W(p.name,p.fileContainerResourceUrl);case 39:void 0!==(j=e.sent)&&(p.files=j.value);case 41:b++,e.next=35;break;case 44:m((function(e){return d.value}));case 45:case"end":return e.stop()}}),e)})))()}),[_,a,w,k]),Object(r.useEffect)((function(){if(void 0!==_&&null!==_&&_.length>0){var e=D(a,_).item;if(null!==e){var t=new EventSource(J+"/"+w+"/"+k+"/_apis/v1/TimeLineWebConsoleLog?timelineId="+e.description),n=[],r=function(t,n){var r=t.find((function(e){return e.id===n.record.StepId})),a=new q.a({newline:!0,escapeXML:!0});return null!=r&&(null==r.log&&(r.log={id:-1,location:null,content:""},n.record.StartLine>1&&Object(S.a)(C.a.mark((function t(){var i,s;return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return console.log("Downloading previous log lines of this step..."),t.next=3,fetch(J+"/"+w+"/"+k+"/_apis/v1/TimeLineWebConsoleLog/"+e.description+"/"+n.record.StepId,{});case 3:if(200!==(i=t.sent).status){t.next=12;break}return t.next=7,i.json();case 7:(s=t.sent).length=n.record.StartLine-1,r.log.content=s.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+a.toHtml(t.line)}),"")+r.log.content,t.next=13;break;case 12:console.log("No logs to download..., currently fixes itself");case 13:case"end":return t.stop()}}),t)})))()),-1===r.log.id&&(r.log.content=n.record.Value.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+a.toHtml(t)}),r.log.content)),!0)};return t.addEventListener("log",(function(e){console.log("new logline "+e.data);var t=JSON.parse(e.data);u((function(e){return r(e,t)?Object(L.a)(e):(n.push(t),e)}))})),t.addEventListener("timeline",(function(e){var t=JSON.parse(e.data);g(t.timeline.shift().name),u((function(e){if(t.timeline.splice(0,e.length),0===t.timeline.length)return e;for(var a=[].concat(Object(L.a)(e),Object(L.a)(t.timeline));n.length>0&&r(a,n[0]);)n.shift();return a}))})),function(){t.close()}}}return function(){}}),[_,a,w,k]),Object(p.jsxs)("section",{className:M.a.component,children:[Object(p.jsx)(j,{title:O}),Object(p.jsx)("main",{className:M.a.main,children:Object(p.jsxs)("div",{className:M.a.text,style:{width:"100%"},children:[T.map((function(e){return Object(p.jsxs)("div",{children:["Error: ",e]})})),f.map((function(e){return Object(p.jsxs)("div",{children:[Object(p.jsx)("div",{children:e.name}),void 0!==e.files?e.files.map((function(e){return Object(p.jsx)("div",{children:Object(p.jsx)("a",{href:e.contentLocation,children:e.path})})})):Object(p.jsx)("div",{})]})})),l.map((function(e){return Object(p.jsx)(H.a,{className:M.a.Collapsible,openedClassName:M.a.Collapsible,triggerClassName:M.a.Collapsible__trigger,triggerOpenedClassName:M.a.Collapsible__trigger+" "+M.a["is-open"],contentOuterClassName:M.a.Collapsible__contentOuter,contentInnerClassName:M.a.Collapsible__contentInner,trigger:e.name,onOpening:function(){e.busy||null!=e.log&&(-1===e.log.id||e.log.content&&0!==e.log.content.length)||(e.busy=!0,Object(S.a)(C.a.mark((function t(){var n,r,i,s,c,o,l;return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:if(t.prev=0,n=new q.a({newline:!0,escapeXML:!0}),null!=e.log){t.next=18;break}return console.log("Downloading previous log lines of this step..."),r=D(a,_).item,t.next=7,fetch(J+"/"+w+"/"+k+"/_apis/v1/TimeLineWebConsoleLog/"+r.description+"/"+e.id,{});case 7:if(200!==(i=t.sent).status){t.next=15;break}return t.next=11,i.json();case 11:s=t.sent,e.log={id:-1,location:null,content:s.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+n.toHtml(t.line)}),"")},t.next=16;break;case 15:console.log("No logs to download...");case 16:t.next=27;break;case 18:return t.next=20,fetch(J+"/"+w+"/"+k+"/_apis/v1/Logfiles/"+e.log.id,{});case 20:return t.next=22,t.sent.text();case 22:c=t.sent,o=c.split("\n"),l="2021-04-02T15:50:14.6619714Z ".length,o[0]=n.toHtml(o[0].substring(l)),e.log.content=o.reduce((function(e,t){return(e.length>0?e+"<br/>":"")+n.toHtml(t.substring(l))}));case 27:return t.prev=27,e.busy=!1,u((function(e){return Object(L.a)(e)})),t.finish(27);case 31:case"end":return t.stop()}}),t,null,[[0,,27,31]])})))())},children:Object(p.jsx)("div",{style:{textAlign:"left",whiteSpace:"nowrap",maxHeight:"100%",overflow:"auto",fontFamily:"SFMono-Regular,Consolas,Liberation Mono,Menlo,monospace"},dangerouslySetInnerHTML:{__html:null!=e.log?e.log.content:"Nothing here"}})},_+e.id)}))]})})]})},X=function(e){var t=Object(o.h)().url,n=Object(r.useState)({items:[]}),i=Object(y.a)(n,2),s=i[0],c=i[1],l=Object(o.g)(),u=l.owner,d=l.repo;return Object(r.useEffect)((function(){var e=J+"/"+u+"/"+d+"/_apis/v1/Message";Object(S.a)(C.a.mark((function t(){return C.a.wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return t.t0=c,t.t1=JSON,t.next=4,fetch(e,{});case 4:return t.next=6,t.sent.text();case 6:t.t2=t.sent,t.t3=t.t1.parse.call(t.t1,t.t2).sort((function(e,t){return t.RequestId-e.RequestId})).map((function(e){return{id:e.RequestId,title:e.name,description:e.workflowname+" - "+e.repo+" - "+e.RequestId}})),t.t4={items:t.t3},(0,t.t0)(t.t4);case 10:case"end":return t.stop()}}),t)})))(),new EventSource(J+"/"+u+"/"+d+"/_apis/v1/Message/event?filter=**").addEventListener("job",(function(e){var t=JSON.parse(e.data).job;c((function(e){return{items:[{id:t.RequestId,title:t.name,description:t.workflowname+" - "+t.repo+" - "+t.RequestId}].concat(Object(L.a)(e.items))}}))}))}),[u,d]),Object(p.jsxs)(a.a.Fragment,{children:[Object(p.jsx)(j,{title:"Jobs",hideBackButton:!0}),Object(p.jsx)("ul",{children:s.items.map((function(e){return Object(p.jsx)("li",{children:Object(p.jsx)(v,{to:"".concat(t,"/detail/").concat(e.id),item:e})},e.id)}))})]})},Z=function(){return Object(p.jsx)(c.a,{children:Object(p.jsxs)(o.d,{children:[Object(p.jsx)(o.b,{path:"/master/:owner/:repo",render:function(e){return Object(p.jsx)(N,{MasterType:X,masterProps:{},DetailType:U,detailProps:{}})}}),Object(p.jsx)(o.a,{exact:!0,from:"/",to:"/master/runner/server"})]})})};Boolean("localhost"===window.location.hostname||"[::1]"===window.location.hostname||window.location.hostname.match(/^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/));s.a.render(Object(p.jsx)(Z,{}),document.getElementById("root")),"serviceWorker"in navigator&&navigator.serviceWorker.ready.then((function(e){e.unregister()}))},9:function(e,t,n){e.exports={component:"Detail_component__1-q_u",main:"Detail_main__2xXle",Collapsible:"Detail_Collapsible__2slk1",Collapsible__contentInner:"Detail_Collapsible__contentInner__3s9IG",Collapsible__trigger:"Detail_Collapsible__trigger__O7Wcx","is-open":"Detail_is-open__2vygp","is-disabled":"Detail_is-disabled__2L3mn","Collapsible__custom-sibling":"Detail_Collapsible__custom-sibling__1TuEJ"}}},[[55,1,2]]]);
//# sourceMappingURL=main.93be3254.chunk.js.map