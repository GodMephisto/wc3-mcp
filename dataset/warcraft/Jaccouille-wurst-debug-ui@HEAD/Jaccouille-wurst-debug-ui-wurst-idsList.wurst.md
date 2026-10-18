---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-debug-ui"
source_path: "wurst/idsList.wurst"
ref: HEAD
---

package idsList

import LinkedList

public let humanUnitList = asList('hpea','hfoo','hgry','hgyr','hkni','hsor','hmtt','hspt',
                                  'hdhw','hmpr','hmtm','hrif','hbot','hbsh','hdes'
                                  )

public let humanHeroList = asList('Hpal', 'Hamg','Hmkg','Hblm')

public let humanBuildingsList = asList('htow','hkee','hcas','hhou','halt','hbar','hbla',
                                       'hlum','harm','hars','hgra','hwtw','hatw','hgtw',
                                       'hctw','hshy','hvlt'
                                       )

public let humanSpecialList = asList('hrtt', 'hmil','hwat','hwt2','hwt3', 'hpxe', 'hphx')

public let orcUnitList = asList('opeo','ogru','orai','otau','ohun','ocat','okod','owyv',
                                'otbr','odoc','oshm','ospw', 'odes', 'obot'
                                )

public let orcHeroList = asList('Obla','Ofar','Otch','Oshd')

public let orcBuildingsList = asList('ogre','ostr','ofrt','oalt','obar','ofor','otto',
                                     'osld','obea','otrb','owtw','ovln', 'oshy'
                                     )

public let orcSpecialList = asList('oeye','otot','osw1','osw2','osw3','ohwd',
                                   'osp1','osp2','osp3','osp4', 'ospm', 'otbk'
                                   )

public let undeadUnitList = asList('uaco','ushd','ugho','uabo','umtw','ucry',
                                   'ugar','uban','unec','uobs','ufro'
                                   )

public let undeadHeroList = asList('Udea', 'Ulic', 'Udre', 'Ucrl')

public let undeadBuildingsList = asList('unpl','unp1','unp2','uzig','uzg1','uzg2','uaod',
                                        'usep','usap','ugrv','utod','ubon','ugol','utom'
                                        )

public let undeadSpecialList = asList('ucrm', 'ugrm','uske','uskm','ubsp','ucs1','ucsB',
                                      'ucs2','ucs3','ucsC', 'uplg', 'uloc'
                                      )

public let nightElfUnitList = asList('ewsp','earc','esen','edry','ebal','ehip','ehpr','echm',
                                     'edot','edoc','emtg', 'efdr'
                                     )

public let nightElfHeroList = asList('Ekee', 'Emoo', 'Edem', 'Ewar')

public let nightElfBuildingsList = asList('etol','etoa','etoe','emow','eate','eaom','eaoe',
                                          'eaow','edob','etrp','edos','egol', 'eden'
                                          )

public let nightElfSpecialList = asList('Edmm', 'edtm','edcm','efon','espv','even',
                                        'nowl','now2','now3'
                                        )

public let artifactList = asList('ckng','desc','modt','ofro','ratf','tkno')

public let campaignList = asList('azhr','bzbe','bzbf','ches','cnhn','dkfw')
                                ..add('dphe','dthb','engs','glsk','gmfr','gopr')
                                ..add('jpnt','k3m1','k3m2','k3m3','ktrm','kybl')
                                ..add('kygh','kymn','kysn','ledg','mgtk','mort')
                                ..add('phlt','sclp','sehr','shwd','skrt','sorf')
                                ..add('thle','wolg','wtlg'
                                )

public let chargedList = asList('ankh','fgdg','fgfh','fgrd','fgrg','fgsk')
                                ..add('hlst','infs','mnst','pdiv','pghe','pgma')
                                ..add('pnvu','pomn','pres','rej3','sand','shar')
                                ..add('sres','sror','srrc','totw','wcyc','whwd')
                                ..add('wild','will','wlsd','woms','wshs','wswd'
                                )

public let miscellaneousList = asList('amrc','anfg','arsc','arsh','asbl')
                                      ..add('axas','bfhr','blba','brag','btst')
                                      ..add('ccmd','cosl','crdt','drph','dtsb')
                                      ..add('dust','envl','esaz','fgun','flag')
                                      ..add('frgd','frhg','fwss','gemt','gldo')
                                      ..add('gobm','grsl','gsou','gvsm','hbth')
                                      ..add('horl','iwbr','jdrn','kgal','klmm')
                                      ..add('lnrn','lure','mlst','mnsf','nflg')
                                      ..add('nspi','ocor','ofir','oflg','ofr2')
                                      ..add('oli2','olig','oslo','oven','pams')
                                      ..add('pgin','pspd','ram1','ram2','ram3'
                                      )

public let miscellaneousList2 = asList('ram4','rat3','rde0','rde1','rej1')
                                       ..add('rej2','rej4','rej5','rej6','rnsp')
                                       ..add('rots','rugt','rump','sbok','schl')
                                       ..add('scul','sfog','shcw','shdt','shen')
                                       ..add('shhn','shrs','shtm','sksh','sor1')
                                       ..add('sor2','sor3','sor4','sor5','sor6')
                                       ..add('sor7','sor8','sor9','sora','soul')
                                       ..add('spre','sprn','srbd','srtl','stpg')
                                       ..add('stre','stwa','tbak','tbar','tbsm')
                                       ..add('tels','tfar','tgxp','thdm','tlum')
                                       ..add('tmmt','tmsc','uflg','vddl','ward'
                                    )


public let permanentList = asList('afac','ajen','belv','bgst','brac','bspd')
                                  ..add('ciri','clfm','clsd','cnob','crys','dsum')
                                  ..add('evtl','gcel','hcun','hval','kpin','lgdh')
                                  ..add('lhst','mcou','odef','penr','pmna','prvt')
                                  ..add('rag1','rat6','rat9','ratc','rde2','rde3')
                                  ..add('rde4','rhth','rin1','rlif','rst1','rwiz')
                                  ..add('sbch','spsh','ssil','stel','war2'
                                  )

public let powerUpList = asList('gfor','gold','gomn','guvi','lmbr','manh')
                                ..add('rdis','rhe1','rhe2','rhe3','rma2','rman')
                                ..add('rre1','rre2','rreb','rres','rspd','rspl')
                                ..add('rsps','rwat','tdex','tdx2','texp','tin2')
                                ..add('tint','tpow','tst2','tstr'
                                )

public let purchasableList = asList('hslv','mcri','moon','pclr','phea','pinv')
                                    ..add('plcl','pman','pnvl','ritd','rnec','shas')
                                    ..add('shea','silk','skul','sman','sneg','spro')
                                    ..add('sreg','ssan','stwp','tcas','tgrh','tret')
                                    ..add('tsct','vamp','wneg','wneu'
                                    )

public let humanUpgradesList = asList('Rhme','Rhra','Rhhb','Rhar','Rhgb','Rhac')
                                      ..add('Rhde','Rhan','Rhpt','Rhst','Rhla','Rhri')
                                      ..add('Rhlh','Rhse','Rhfl','Rhss','Rhrt','Rhpm')
                                      ..add('Rhfc','Rhfs','Rhcd','Rhsb'
                                      )

public let orcUpgradesList = asList('Rome','Rora','Roar','Rwdm','Ropg','Robs')
                                    ..add('Rows','Roen','Rovs','Rowd','Rost','Rosp')
                                    ..add('Rotr','Rolf','Roch','Rowt','Rorb','Robk')
                                    ..add('Ropm','Robf'
                                    )

public let undeadUpgradesList = asList('Rume','Rura','Ruar','Ruac','Rugf','Ruwb')
                                       ..add('Rusf','Rune','Ruba','Rufb','Rusl','Rucr')
                                       ..add('Rupc','Rusm','Rubu','Rusp','Ruex','Rupm'
                                       )

public let nightElfUpgradesList = asList('Resm','Resw','Rema','Rerh','Reuv','Renb')
                                         ..add('Resc','Remg','Reib','Remk','Redt','Redc')
                                         ..add('Resi','Recb','Reht','Repb','Rers','Rehs')
                                         ..add('Reeb','Reec','Rews','Repm'
                                         )

public let specialUpgradesList = asList('Rgfo','Rguv','Rnen','Rnsw','Rnsi','Rnat','Rnam','Rnsb')
